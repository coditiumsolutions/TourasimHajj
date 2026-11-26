using System.Globalization;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using TourismHajj.Helpers;
using TourismHajj.Interfaces;
using TourismHajj.ViewModels;

namespace TourismHajj.Services.Amadeus;

public class AmadeusProvider : IFlightProvider
{
    private readonly HttpClient _http;
    private readonly AmadeusOptions _options;
    private readonly ILogger<AmadeusProvider> _logger;
    private string? _accessToken;
    private DateTime _tokenExpiresUtc = DateTime.MinValue;
    private readonly Dictionary<string, FlightOfferItem> _offerCache = new(StringComparer.Ordinal);
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };

    public AmadeusProvider(HttpClient http, IOptions<AmadeusOptions> options, ILogger<AmadeusProvider> logger)
    {
        _http = http;
        _options = options.Value;
        _logger = logger;
    }

    public string ProviderName => "Amadeus";
    public bool IsConfigured => _options.IsConfigured;

    public async Task<IReadOnlyList<FlightOfferItem>> SearchAsync(FlightSearchRequest request, CancellationToken cancellationToken = default)
    {
        if (!IsConfigured)
            return Array.Empty<FlightOfferItem>();

        if (!await EnsureTokenAsync(cancellationToken))
            return Array.Empty<FlightOfferItem>();

        var query = new Dictionary<string, string?>
        {
            ["originLocationCode"] = request.Origin.Trim().ToUpperInvariant(),
            ["destinationLocationCode"] = request.Destination.Trim().ToUpperInvariant(),
            ["departureDate"] = request.DepartureDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            ["adults"] = Math.Max(1, request.Adults).ToString(CultureInfo.InvariantCulture),
            ["max"] = Math.Clamp(_options.MaxOffers, 1, 50).ToString(CultureInfo.InvariantCulture),
            ["currencyCode"] = "BHD"
        };
        if (!request.OneWay && request.ReturnDate.HasValue)
            query["returnDate"] = request.ReturnDate.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        if (request.Children > 0)
            query["children"] = request.Children.ToString(CultureInfo.InvariantCulture);

        var qs = string.Join("&", query
            .Where(kv => !string.IsNullOrWhiteSpace(kv.Value))
            .Select(kv => $"{Uri.EscapeDataString(kv.Key)}={Uri.EscapeDataString(kv.Value!)}"));

        using var req = new HttpRequestMessage(HttpMethod.Get, "/v2/shopping/flight-offers?" + qs);
        req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);

        using var res = await _http.SendAsync(req, cancellationToken);
        var json = await res.Content.ReadAsStringAsync(cancellationToken);
        if (!res.IsSuccessStatusCode)
        {
            _logger.LogWarning("Amadeus search failed {Status}: {Body}", (int)res.StatusCode, Truncate(json));
            return Array.Empty<FlightOfferItem>();
        }

        using var doc = JsonDocument.Parse(json);
        var results = new List<FlightOfferItem>();
        if (!doc.RootElement.TryGetProperty("data", out var data) || data.ValueKind != JsonValueKind.Array)
            return results;

        var carriers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        if (doc.RootElement.TryGetProperty("dictionaries", out var dicts) &&
            dicts.TryGetProperty("carriers", out var carrierNode))
        {
            foreach (var c in carrierNode.EnumerateObject())
                carriers[c.Name] = c.Value.GetString() ?? c.Name;
        }

        foreach (var offer in data.EnumerateArray())
        {
            var item = MapOffer(offer, carriers);
            if (item is null) continue;
            _offerCache[item.OfferId] = item;
            results.Add(item);
        }

        return results.OrderBy(r => r.TotalPrice).ToList();
    }

    public Task<FlightOfferItem?> GetOfferAsync(string offerId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(offerId))
            return Task.FromResult<FlightOfferItem?>(null);
        _offerCache.TryGetValue(offerId, out var item);
        return Task.FromResult(item);
    }

    public Task<FlightProviderBookingResult> BookAsync(FlightBookRequest request, CancellationToken cancellationToken = default)
    {
        // Full Amadeus Flight Create Orders requires offer pricing + traveler docs.
        // Until live keys + order flow are enabled, surface a clear message for live search-only mode.
        return Task.FromResult(new FlightProviderBookingResult
        {
            Success = false,
            ErrorMessage = "Live Amadeus ticketing is not enabled yet. Use demo mode (empty keys) for end-to-end book/ticket, or complete Flight Create Orders setup with your Amadeus app."
        });
    }

    private FlightOfferItem? MapOffer(JsonElement offer, IReadOnlyDictionary<string, string> carriers)
    {
        if (!offer.TryGetProperty("id", out var idEl))
            return null;

        var offerId = idEl.GetString() ?? Guid.NewGuid().ToString("N");
        if (!offer.TryGetProperty("itineraries", out var itineraries) || itineraries.GetArrayLength() == 0)
            return null;

        var outbound = itineraries[0];
        if (!outbound.TryGetProperty("segments", out var segments) || segments.GetArrayLength() == 0)
            return null;

        var first = segments[0];
        var last = segments[segments.GetArrayLength() - 1];
        var carrier = first.TryGetProperty("carrierCode", out var cc) ? cc.GetString() ?? "" : "";
        var number = first.TryGetProperty("number", out var num) ? num.GetString() ?? "" : "";
        var depAt = ParseAmadeusDate(first.GetProperty("departure").GetProperty("at").GetString());
        var arrAt = ParseAmadeusDate(last.GetProperty("arrival").GetProperty("at").GetString());
        var origin = first.GetProperty("departure").GetProperty("iataCode").GetString() ?? "";
        var dest = last.GetProperty("arrival").GetProperty("iataCode").GetString() ?? "";

        decimal price = 0;
        var currency = "BHD";
        if (offer.TryGetProperty("price", out var priceNode))
        {
            if (priceNode.TryGetProperty("grandTotal", out var gt) &&
                decimal.TryParse(gt.GetString(), NumberStyles.Any, CultureInfo.InvariantCulture, out var p))
                price = p;
            if (priceNode.TryGetProperty("currency", out var cur))
                currency = cur.GetString() ?? currency;
        }

        var item = new FlightOfferItem
        {
            OfferId = offerId,
            CarrierCode = carrier,
            CarrierName = carriers.TryGetValue(carrier, out var name) ? name : carrier,
            FlightNumber = string.IsNullOrWhiteSpace(carrier) ? number : carrier + number,
            Origin = origin,
            Destination = dest,
            DepartureAt = depAt,
            ArrivalAt = arrAt,
            Duration = outbound.TryGetProperty("duration", out var dur) ? FormatIsoDuration(dur.GetString()) : "",
            Stops = Math.Max(0, segments.GetArrayLength() - 1),
            Cabin = "ECONOMY",
            TotalPrice = price,
            Currency = currency,
            IsRoundTrip = itineraries.GetArrayLength() > 1,
            Summary = $"{carrier}{number} · {origin}→{dest}"
        };

        if (itineraries.GetArrayLength() > 1)
        {
            var inbound = itineraries[1];
            if (inbound.TryGetProperty("segments", out var inSegs) && inSegs.GetArrayLength() > 0)
            {
                var inFirst = inSegs[0];
                var inLast = inSegs[inSegs.GetArrayLength() - 1];
                var inCarrier = inFirst.TryGetProperty("carrierCode", out var ic) ? ic.GetString() ?? "" : "";
                var inNumber = inFirst.TryGetProperty("number", out var inn) ? inn.GetString() ?? "" : "";
                item.ReturnFlightNumber = inCarrier + inNumber;
                item.ReturnDepartureAt = ParseAmadeusDate(inFirst.GetProperty("departure").GetProperty("at").GetString());
                item.ReturnArrivalAt = ParseAmadeusDate(inLast.GetProperty("arrival").GetProperty("at").GetString());
            }
        }

        return item;
    }

    private async Task<bool> EnsureTokenAsync(CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(_accessToken) && DateTime.UtcNow < _tokenExpiresUtc.AddMinutes(-2))
            return true;

        var body = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["grant_type"] = "client_credentials",
            ["client_id"] = _options.ClientId,
            ["client_secret"] = _options.ClientSecret
        });

        using var res = await _http.PostAsync("/v1/security/oauth2/token", body, cancellationToken);
        var json = await res.Content.ReadAsStringAsync(cancellationToken);
        if (!res.IsSuccessStatusCode)
        {
            _logger.LogWarning("Amadeus token failed {Status}: {Body}", (int)res.StatusCode, Truncate(json));
            return false;
        }

        using var doc = JsonDocument.Parse(json);
        _accessToken = doc.RootElement.GetProperty("access_token").GetString();
        var expiresIn = doc.RootElement.TryGetProperty("expires_in", out var exp) ? exp.GetInt32() : 1799;
        _tokenExpiresUtc = DateTime.UtcNow.AddSeconds(expiresIn);
        return !string.IsNullOrWhiteSpace(_accessToken);
    }

    private static DateTime ParseAmadeusDate(string? value) =>
        DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var dt)
            ? dt
            : DateTime.MinValue;

    private static string FormatIsoDuration(string? iso)
    {
        if (string.IsNullOrWhiteSpace(iso)) return "";
        // PT5H30M
        iso = iso.Trim().TrimStart('P', 'T');
        var hours = 0;
        var minutes = 0;
        var hIdx = iso.IndexOf('H');
        if (hIdx > 0 && int.TryParse(iso[..hIdx], out var h))
        {
            hours = h;
            iso = iso[(hIdx + 1)..];
        }
        var mIdx = iso.IndexOf('M');
        if (mIdx > 0 && int.TryParse(iso[..mIdx], out var m))
            minutes = m;
        return $"{hours}h {minutes:00}m";
    }

    private static string Truncate(string s) => s.Length <= 400 ? s : s[..400];
}
