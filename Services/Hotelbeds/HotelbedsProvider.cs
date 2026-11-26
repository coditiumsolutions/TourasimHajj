using System.Globalization;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using TourismHajj.Helpers;
using TourismHajj.Interfaces;
using TourismHajj.ViewModels;

namespace TourismHajj.Services.Hotelbeds;

public class HotelbedsProvider : IHotelProvider
{
    private readonly HttpClient _http;
    private readonly HotelbedsOptions _options;
    private readonly ILogger<HotelbedsProvider> _logger;
    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public HotelbedsProvider(HttpClient http, IOptions<HotelbedsOptions> options, ILogger<HotelbedsProvider> logger)
    {
        _http = http;
        _options = options.Value;
        _logger = logger;
    }

    public string ProviderName => "Hotelbeds";
    public bool IsConfigured => _options.IsConfigured;

    public async Task<IReadOnlyList<HotelSearchResultItem>> SearchAsync(HotelSearchRequest request, CancellationToken cancellationToken = default)
    {
        if (!IsConfigured)
            return Array.Empty<HotelSearchResultItem>();

        var destinationCode = ResolveDestinationCode(request.Destination);
        var body = new Dictionary<string, object?>
        {
            ["stay"] = new
            {
                checkIn = request.CheckIn.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                checkOut = request.CheckOut.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
            },
            ["occupancies"] = new[]
            {
                new
                {
                    rooms = request.Rooms,
                    adults = request.Adults,
                    children = request.Children
                }
            },
            ["destination"] = new { code = destinationCode },
            ["filter"] = new
            {
                maxHotels = _options.MaxHotels,
                maxRooms = 5,
                minCategory = request.MinStars
            }
        };

        using var doc = await SendJsonAsync(HttpMethod.Post, "/hotel-api/1.0/hotels", body, cancellationToken);
        if (doc is null)
            return Array.Empty<HotelSearchResultItem>();

        var results = new List<HotelSearchResultItem>();
        if (!doc.RootElement.TryGetProperty("hotels", out var hotelsNode) ||
            !hotelsNode.TryGetProperty("hotels", out var hotelArray) ||
            hotelArray.ValueKind != JsonValueKind.Array)
        {
            return results;
        }

        foreach (var hotel in hotelArray.EnumerateArray())
        {
            var item = new HotelSearchResultItem
            {
                HotelCode = GetString(hotel, "code") ?? string.Empty,
                HotelName = GetString(hotel, "name") ?? "Hotel",
                DestinationName = GetNestedString(hotel, "destinationName") ?? request.Destination,
                ZoneName = GetNestedString(hotel, "zoneName"),
                CategoryName = GetString(hotel, "categoryName"),
                CategoryCode = ParseStars(GetString(hotel, "categoryCode") ?? GetString(hotel, "categoryName")),
                MinRate = ParseDecimal(GetString(hotel, "minRate")),
                Currency = GetString(hotel, "currency") ?? "USD"
            };

            if (hotel.TryGetProperty("rooms", out var rooms) && rooms.ValueKind == JsonValueKind.Array)
            {
                foreach (var room in rooms.EnumerateArray())
                {
                    var roomName = GetString(room, "name") ?? "Room";
                    var roomCode = GetString(room, "code") ?? string.Empty;
                    if (!room.TryGetProperty("rates", out var rates) || rates.ValueKind != JsonValueKind.Array)
                        continue;

                    foreach (var rate in rates.EnumerateArray())
                    {
                        var rateKey = GetString(rate, "rateKey");
                        if (string.IsNullOrWhiteSpace(rateKey))
                            continue;

                        var net = ParseDecimal(GetString(rate, "net"));
                        var selling = ParseDecimal(GetString(rate, "sellingRate"));
                        if (selling <= 0) selling = net;

                        item.Rooms.Add(new HotelRoomOffer
                        {
                            RateKey = rateKey,
                            RoomCode = roomCode,
                            RoomName = roomName,
                            BoardCode = GetString(rate, "boardCode"),
                            BoardName = GetString(rate, "boardName"),
                            Net = net,
                            SellingRate = selling,
                            Currency = item.Currency,
                            RateCommentsRequired = string.Equals(GetString(rate, "rateType"), "RECHECK", StringComparison.OrdinalIgnoreCase),
                            CancellationPolicySummary = BuildCancellationSummary(rate),
                            Adults = request.Adults,
                            Children = request.Children
                        });
                    }
                }
            }

            if (item.Rooms.Count == 0)
                continue;

            if (item.MinRate <= 0)
                item.MinRate = item.Rooms.Min(r => r.SellingRate);

            results.Add(item);
        }

        return results
            .OrderBy(r => r.MinRate)
            .ThenByDescending(r => r.CategoryCode ?? 0)
            .ToList();
    }

    public async Task<HotelRateCheckResult?> CheckRateAsync(string rateKey, CancellationToken cancellationToken = default)
    {
        if (!IsConfigured || string.IsNullOrWhiteSpace(rateKey))
            return null;

        var body = new
        {
            rooms = new[] { new { rateKey } }
        };

        using var doc = await SendJsonAsync(HttpMethod.Post, "/hotel-api/1.0/checkrates", body, cancellationToken);
        if (doc is null)
            return null;

        if (!doc.RootElement.TryGetProperty("hotel", out var hotel))
            return null;

        var room = hotel.TryGetProperty("rooms", out var rooms) && rooms.GetArrayLength() > 0
            ? rooms[0]
            : default;
        var rate = room.ValueKind == JsonValueKind.Object &&
                   room.TryGetProperty("rates", out var rates) && rates.GetArrayLength() > 0
            ? rates[0]
            : default;

        var newRateKey = rate.ValueKind == JsonValueKind.Object ? GetString(rate, "rateKey") : rateKey;
        var net = rate.ValueKind == JsonValueKind.Object ? ParseDecimal(GetString(rate, "net")) : 0;
        var selling = rate.ValueKind == JsonValueKind.Object ? ParseDecimal(GetString(rate, "sellingRate")) : 0;
        if (selling <= 0) selling = net;

        return new HotelRateCheckResult
        {
            RateKey = newRateKey ?? rateKey,
            HotelCode = GetString(hotel, "code") ?? string.Empty,
            HotelName = GetString(hotel, "name") ?? "Hotel",
            RoomName = room.ValueKind == JsonValueKind.Object ? (GetString(room, "name") ?? "Room") : "Room",
            BoardName = rate.ValueKind == JsonValueKind.Object ? GetString(rate, "boardName") : null,
            Net = net,
            SellingRate = selling,
            Currency = GetString(hotel, "currency") ?? "USD",
            CancellationPolicySummary = rate.ValueKind == JsonValueKind.Object ? BuildCancellationSummary(rate) : null,
            RateComments = rate.ValueKind == JsonValueKind.Object ? GetString(rate, "rateComments") : null,
            CheckIn = ParseDate(GetString(hotel, "checkIn")) ?? DateTime.Today,
            CheckOut = ParseDate(GetString(hotel, "checkOut")) ?? DateTime.Today.AddDays(1)
        };
    }

    public async Task<HotelProviderBookingResult> BookAsync(HotelBookRequest request, CancellationToken cancellationToken = default)
    {
        if (!IsConfigured)
        {
            return new HotelProviderBookingResult
            {
                Success = false,
                ErrorMessage = "Hotelbeds API is not configured. Add ApiKey and Secret in appsettings / user secrets."
            };
        }

        var paxes = request.Guests.Select(g => new Dictionary<string, object?>
        {
            ["roomId"] = 1,
            ["type"] = g.Type,
            ["name"] = g.FirstName,
            ["surname"] = g.LastName,
            ["age"] = g.Type == "CH" ? g.Age : null
        }).ToList();

        if (paxes.Count == 0)
        {
            paxes.Add(new Dictionary<string, object?>
            {
                ["roomId"] = 1,
                ["type"] = "AD",
                ["name"] = request.HolderFirstName,
                ["surname"] = request.HolderLastName
            });
        }

        var body = new Dictionary<string, object?>
        {
            ["holder"] = new
            {
                name = request.HolderFirstName,
                surname = request.HolderLastName,
                email = request.HolderEmail,
                phoneNumber = request.HolderPhone
            },
            ["rooms"] = new[]
            {
                new
                {
                    rateKey = request.RateKey,
                    paxes
                }
            },
            ["clientReference"] = string.IsNullOrWhiteSpace(request.ClientReference)
                ? Guid.NewGuid().ToString("N")[..12].ToUpperInvariant()
                : request.ClientReference,
            ["remark"] = request.Remark,
            ["tolerance"] = 2
        };

        using var doc = await SendJsonAsync(HttpMethod.Post, "/hotel-api/1.0/bookings", body, cancellationToken);
        if (doc is null)
        {
            return new HotelProviderBookingResult
            {
                Success = false,
                ErrorMessage = "No response from Hotelbeds booking API."
            };
        }

        var root = doc.RootElement;
        if (root.TryGetProperty("error", out var error))
        {
            return new HotelProviderBookingResult
            {
                Success = false,
                ErrorMessage = GetString(error, "message") ?? error.ToString(),
                RawJson = root.GetRawText()
            };
        }

        if (!root.TryGetProperty("booking", out var booking))
        {
            return new HotelProviderBookingResult
            {
                Success = false,
                ErrorMessage = "Unexpected booking response from Hotelbeds.",
                RawJson = root.GetRawText()
            };
        }

        string? hotelCode = null;
        string? hotelName = null;
        if (booking.TryGetProperty("hotel", out var hotel))
        {
            hotelCode = GetString(hotel, "code");
            hotelName = GetString(hotel, "name");
        }

        return new HotelProviderBookingResult
        {
            Success = true,
            SupplierBookingReference = GetString(booking, "reference"),
            Status = GetString(booking, "status") ?? "CONFIRMED",
            TotalNet = ParseDecimal(GetString(booking, "totalNet")),
            TotalSelling = ParseDecimal(GetString(booking, "totalSellingRate")),
            Currency = GetString(booking, "currency"),
            HotelCode = hotelCode,
            HotelName = hotelName,
            RawJson = root.GetRawText()
        };
    }

    public async Task<bool> CancelAsync(string supplierBookingReference, CancellationToken cancellationToken = default)
    {
        if (!IsConfigured || string.IsNullOrWhiteSpace(supplierBookingReference))
            return false;

        using var doc = await SendJsonAsync(
            HttpMethod.Delete,
            $"/hotel-api/1.0/bookings/{Uri.EscapeDataString(supplierBookingReference)}?cancellationFlag=CANCELLATION",
            null,
            cancellationToken);

        return doc is not null && !doc.RootElement.TryGetProperty("error", out _);
    }

    private string ResolveDestinationCode(string destination)
    {
        if (destination.Contains("Madin", StringComparison.OrdinalIgnoreCase) ||
            destination.Contains("Medin", StringComparison.OrdinalIgnoreCase))
            return _options.MadinahDestinationCode;

        return _options.MakkahDestinationCode;
    }

    private async Task<JsonDocument?> SendJsonAsync(HttpMethod method, string path, object? body, CancellationToken cancellationToken)
    {
        try
        {
            using var request = new HttpRequestMessage(method, path.TrimStart('/'));
            ApplyAuth(request);
            if (body is not null)
            {
                var json = JsonSerializer.Serialize(body, JsonOpts);
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            }

            using var response = await _http.SendAsync(request, cancellationToken);
            var payload = await response.Content.ReadAsStringAsync(cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Hotelbeds {Method} {Path} failed: {Status} {Body}",
                    method, path, (int)response.StatusCode, Truncate(payload, 500));
                if (!string.IsNullOrWhiteSpace(payload))
                {
                    try { return JsonDocument.Parse(payload); }
                    catch { /* ignore */ }
                }
                return null;
            }

            if (string.IsNullOrWhiteSpace(payload))
                return null;

            return JsonDocument.Parse(payload);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Hotelbeds request failed for {Path}", path);
            return null;
        }
    }

    private void ApplyAuth(HttpRequestMessage request)
    {
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture);
        var raw = _options.ApiKey + _options.Secret + timestamp;
        var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(raw))).ToLowerInvariant();

        request.Headers.TryAddWithoutValidation("Api-key", _options.ApiKey);
        request.Headers.TryAddWithoutValidation("X-Signature", hash);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        request.Headers.TryAddWithoutValidation("Accept-Encoding", "gzip");
    }

    private static string? GetString(JsonElement el, string name) =>
        el.TryGetProperty(name, out var p) && p.ValueKind == JsonValueKind.String ? p.GetString() :
        el.TryGetProperty(name, out p) && p.ValueKind is JsonValueKind.Number ? p.ToString() : null;

    private static string? GetNestedString(JsonElement el, string name)
    {
        if (!el.TryGetProperty(name, out var p))
            return null;
        if (p.ValueKind == JsonValueKind.String)
            return p.GetString();
        if (p.ValueKind == JsonValueKind.Object && p.TryGetProperty("content", out var c))
            return c.GetString();
        return p.ToString();
    }

    private static decimal ParseDecimal(string? value) =>
        decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var d) ? d : 0m;

    private static DateTime? ParseDate(string? value) =>
        DateTime.TryParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var d) ? d : null;

    private static int? ParseStars(string? category)
    {
        if (string.IsNullOrWhiteSpace(category))
            return null;
        var digits = new string(category.Where(char.IsDigit).ToArray());
        return int.TryParse(digits, out var n) && n is >= 1 and <= 5 ? n : null;
    }

    private static string? BuildCancellationSummary(JsonElement rate)
    {
        if (!rate.TryGetProperty("cancellationPolicies", out var policies) || policies.ValueKind != JsonValueKind.Array)
            return null;

        var parts = new List<string>();
        foreach (var p in policies.EnumerateArray().Take(2))
        {
            var amount = GetString(p, "amount");
            var from = GetString(p, "from");
            if (!string.IsNullOrWhiteSpace(amount) || !string.IsNullOrWhiteSpace(from))
                parts.Add($"From {from}: {amount}");
        }

        return parts.Count == 0 ? null : string.Join("; ", parts);
    }

    private static string Truncate(string value, int max) =>
        value.Length <= max ? value : value[..max] + "...";
}
