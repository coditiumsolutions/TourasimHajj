using System.Globalization;
using TourismHajj.Interfaces;
using TourismHajj.ViewModels;

namespace TourismHajj.Services.Amadeus;

/// <summary>
/// Sample Bahrain ↔ Jeddah/Madinah offers when Amadeus keys are not configured.
/// Offer ids are prefixed with DEMO| so book can round-trip without the external API.
/// </summary>
public class DemoFlightProvider : IFlightProvider
{
    public string ProviderName => "Demo";
    public bool IsConfigured => true;

    public Task<IReadOnlyList<FlightOfferItem>> SearchAsync(FlightSearchRequest request, CancellationToken cancellationToken = default)
    {
        var origin = (request.Origin ?? "BAH").Trim().ToUpperInvariant();
        var dest = (request.Destination ?? "JED").Trim().ToUpperInvariant();
        var depDate = request.DepartureDate.Date;
        DateTime? retDate = request.OneWay
            ? null
            : (request.ReturnDate?.Date ?? depDate.AddDays(7));
        var pax = Math.Max(1, request.Adults + request.Children);

        var catalog = BuildCatalog(origin, dest, depDate, retDate, pax);
        return Task.FromResult<IReadOnlyList<FlightOfferItem>>(catalog);
    }

    public async Task<FlightOfferItem?> GetOfferAsync(string offerId, CancellationToken cancellationToken = default)
    {
        if (!TryParseOfferId(offerId, out var parsed))
            return null;

        var request = new FlightSearchRequest
        {
            Origin = parsed.Origin,
            Destination = parsed.Destination,
            DepartureDate = parsed.DepartureDate,
            ReturnDate = parsed.ReturnDate,
            OneWay = parsed.ReturnDate is null,
            Adults = 1
        };
        var results = await SearchAsync(request, cancellationToken);
        return results.FirstOrDefault(r => r.OfferId == offerId)
               ?? results.FirstOrDefault(r => r.FlightNumber == parsed.FlightNumber);
    }

    public Task<FlightProviderBookingResult> BookAsync(FlightBookRequest request, CancellationToken cancellationToken = default)
    {
        if (!TryParseOfferId(request.OfferId, out var parsed))
        {
            return Task.FromResult(new FlightProviderBookingResult
            {
                Success = false,
                ErrorMessage = "Demo offer expired or invalid. Please search again."
            });
        }

        var reference = "DEMO-FLT-" + DateTime.UtcNow.ToString("yyMMddHHmmss", CultureInfo.InvariantCulture);
        var ticket = "ETKT" + DateTime.UtcNow.ToString("yyyyMMddHHmm", CultureInfo.InvariantCulture);
        return Task.FromResult(new FlightProviderBookingResult
        {
            Success = true,
            SupplierBookingReference = reference,
            TicketNumber = ticket,
            Status = "TICKETED",
            TotalPrice = parsed.Price,
            Currency = parsed.Currency,
            RawJson = $"{{\"demo\":true,\"reference\":\"{reference}\",\"ticket\":\"{ticket}\"}}"
        });
    }

    private static List<FlightOfferItem> BuildCatalog(
        string origin, string dest, DateTime depDate, DateTime? retDate, int pax)
    {
        var samples = new (string Carrier, string Name, string Number, int DepHour, int ArrHour, int Stops, decimal Base)[]
        {
            ("GF", "Gulf Air", "GF171", 8, 10, 0, 95m),
            ("SV", "Saudia", "SV523", 11, 13, 0, 110m),
            ("F3", "flyadeal", "F3124", 14, 16, 0, 78m),
            ("XY", "flynas", "XY210", 17, 20, 1, 72m),
            ("EK", "Emirates", "EK838", 6, 12, 1, 145m)
        };

        var list = new List<FlightOfferItem>();
        foreach (var s in samples)
        {
            var dep = depDate.AddHours(s.DepHour);
            var arr = depDate.AddHours(s.ArrHour);
            if (arr <= dep) arr = arr.AddDays(1);
            var price = Math.Round(s.Base * pax * (retDate.HasValue ? 1.85m : 1m), 2);
            var offerId = BuildOfferId(origin, dest, s.Number, depDate, retDate, price, "BHD");

            var item = new FlightOfferItem
            {
                OfferId = offerId,
                CarrierCode = s.Carrier,
                CarrierName = s.Name,
                FlightNumber = s.Number,
                Origin = origin,
                Destination = dest,
                DepartureAt = dep,
                ArrivalAt = arr,
                Duration = FormatDuration(arr - dep),
                Stops = s.Stops,
                Cabin = "ECONOMY",
                TotalPrice = price,
                Currency = "BHD",
                IsRoundTrip = retDate.HasValue,
                Summary = $"{s.Name} {s.Number} · {origin}→{dest}" + (s.Stops == 0 ? " · Non-stop" : $" · {s.Stops} stop")
            };

            if (retDate.HasValue)
            {
                item.ReturnFlightNumber = s.Number.Replace("1", "2", StringComparison.Ordinal);
                item.ReturnDepartureAt = retDate.Value.AddHours(s.DepHour + 1);
                item.ReturnArrivalAt = retDate.Value.AddHours(s.ArrHour + 1);
                item.Summary += $" · return {retDate:dd MMM}";
            }

            list.Add(item);
        }

        return list.OrderBy(x => x.TotalPrice).ToList();
    }

    private static string BuildOfferId(
        string origin, string dest, string flightNumber, DateTime dep, DateTime? ret, decimal price, string currency) =>
        string.Join('|',
            "DEMO",
            origin,
            dest,
            flightNumber,
            dep.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            ret?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) ?? "",
            price.ToString("0.00", CultureInfo.InvariantCulture),
            currency);

    private static bool TryParseOfferId(string offerId, out (
        string Origin, string Destination, string FlightNumber, DateTime DepartureDate, DateTime? ReturnDate, decimal Price, string Currency) parsed)
    {
        parsed = default;
        if (string.IsNullOrWhiteSpace(offerId) || !offerId.StartsWith("DEMO|", StringComparison.Ordinal))
            return false;

        var parts = offerId.Split('|');
        if (parts.Length < 8)
            return false;
        if (!DateTime.TryParseExact(parts[4], "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dep))
            return false;
        DateTime? ret = null;
        if (!string.IsNullOrWhiteSpace(parts[5]))
        {
            if (!DateTime.TryParseExact(parts[5], "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var r))
                return false;
            ret = r;
        }
        if (!decimal.TryParse(parts[6], NumberStyles.Any, CultureInfo.InvariantCulture, out var price))
            return false;

        parsed = (parts[1], parts[2], parts[3], dep, ret, price, parts[7]);
        return true;
    }

    private static string FormatDuration(TimeSpan span) =>
        $"{(int)span.TotalHours}h {span.Minutes:00}m";
}
