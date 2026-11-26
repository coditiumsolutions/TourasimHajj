using System.Globalization;
using TourismHajj.Interfaces;
using TourismHajj.ViewModels;

namespace TourismHajj.Services.Hotelbeds;

/// <summary>
/// Sample Makkah/Madinah inventory used on the public /Hotels page when Hotelbeds keys are not configured.
/// Rate keys are prefixed with DEMO| so check/book can round-trip without an external API.
/// </summary>
public class DemoHotelProvider : IHotelProvider
{
    public string ProviderName => "Demo";
    public bool IsConfigured => true;

    public Task<IReadOnlyList<HotelSearchResultItem>> SearchAsync(HotelSearchRequest request, CancellationToken cancellationToken = default)
    {
        var nights = Math.Max(1, (request.CheckOut.Date - request.CheckIn.Date).Days);
        var isMadinah = request.Destination.Contains("Madin", StringComparison.OrdinalIgnoreCase) ||
                        request.Destination.Contains("Medin", StringComparison.OrdinalIgnoreCase);

        var catalog = isMadinah
            ? new[]
            {
                ("MED001", "Anwar Al Madinah Mövenpick", 5, 185m, "0.3 km from Masjid an-Nabawi"),
                ("MED002", "Pullman Zamzam Madina", 5, 160m, "Walking distance to Haram"),
                ("MED003", "Oberoi Madina", 5, 220m, "Luxury stay near Nabawi"),
                ("MED004", "Shaza Al Madina", 4, 95m, "Family rooms near Haram"),
                ("MED005", "Millennium Al Aqeeq Madinah", 4, 110m, "Comfort stay for Umrah")
            }
            : new[]
            {
                ("MKK001", "Swissôtel Makkah", 5, 175m, "Clock Tower / Abraj Al Bait area"),
                ("MKK002", "Hilton Suites Makkah", 5, 155m, "Short walk to Masjid al-Haram"),
                ("MKK003", "Makkah Hotel", 4, 120m, "Haram view rooms available"),
                ("MKK004", "Pullman Zamzam Makkah", 5, 190m, "Adjacent to Haram"),
                ("MKK005", "Al Safwah Royale Orchid", 4, 88m, "Value stay near Haram"),
                ("MKK006", "Anjum Hotel Makkah", 4, 105m, "Family-friendly Umrah hotel")
            };

        var results = new List<HotelSearchResultItem>();
        foreach (var (code, name, stars, nightly, zone) in catalog)
        {
            var total = Math.Round(nightly * nights * Math.Max(1, request.Rooms), 2);
            var rateKeyBb = BuildRateKey(code, name, "BB", "Bed & Breakfast", total, request);
            var rateKeyRo = BuildRateKey(code, name, "RO", "Room Only", Math.Round(total * 0.9m, 2), request);

            results.Add(new HotelSearchResultItem
            {
                HotelCode = code,
                HotelName = name,
                DestinationName = isMadinah ? "Madinah" : "Makkah",
                ZoneName = zone,
                CategoryCode = stars,
                CategoryName = $"{stars} STARS",
                MinRate = Math.Round(total * 0.9m, 2),
                Currency = "USD",
                Rooms =
                {
                    new HotelRoomOffer
                    {
                        RateKey = rateKeyRo,
                        RoomCode = "DBL",
                        RoomName = "Double Room",
                        BoardCode = "RO",
                        BoardName = "Room Only",
                        Net = Math.Round(total * 0.85m, 2),
                        SellingRate = Math.Round(total * 0.9m, 2),
                        Currency = "USD",
                        CancellationPolicySummary = "Free cancellation until 48h before check-in (demo)",
                        Adults = request.Adults,
                        Children = request.Children
                    },
                    new HotelRoomOffer
                    {
                        RateKey = rateKeyBb,
                        RoomCode = "DBL-BB",
                        RoomName = "Double Room",
                        BoardCode = "BB",
                        BoardName = "Bed & Breakfast",
                        Net = Math.Round(total * 0.95m, 2),
                        SellingRate = total,
                        Currency = "USD",
                        CancellationPolicySummary = "Non-refundable after booking (demo)",
                        Adults = request.Adults,
                        Children = request.Children
                    }
                }
            });
        }

        return Task.FromResult<IReadOnlyList<HotelSearchResultItem>>(
            results.OrderBy(r => r.MinRate).ToList());
    }

    public Task<HotelRateCheckResult?> CheckRateAsync(string rateKey, CancellationToken cancellationToken = default)
    {
        if (!TryParseRateKey(rateKey, out var parsed))
            return Task.FromResult<HotelRateCheckResult?>(null);

        return Task.FromResult<HotelRateCheckResult?>(new HotelRateCheckResult
        {
            RateKey = rateKey,
            HotelCode = parsed.HotelCode,
            HotelName = parsed.HotelName,
            RoomName = "Double Room",
            BoardName = parsed.BoardName,
            Net = Math.Round(parsed.Selling * 0.92m, 2),
            SellingRate = parsed.Selling,
            Currency = "USD",
            CancellationPolicySummary = "Demo rate — replace with Hotelbeds keys for live inventory",
            CheckIn = parsed.CheckIn,
            CheckOut = parsed.CheckOut
        });
    }

    public Task<HotelProviderBookingResult> BookAsync(HotelBookRequest request, CancellationToken cancellationToken = default)
    {
        if (!TryParseRateKey(request.RateKey, out var parsed))
        {
            return Task.FromResult(new HotelProviderBookingResult
            {
                Success = false,
                ErrorMessage = "Demo rate key is invalid or expired. Please search again."
            });
        }

        var reference = "DEMO-" + DateTime.UtcNow.ToString("yyMMddHHmmss", CultureInfo.InvariantCulture);
        return Task.FromResult(new HotelProviderBookingResult
        {
            Success = true,
            SupplierBookingReference = reference,
            Status = "CONFIRMED",
            TotalNet = Math.Round(parsed.Selling * 0.92m, 2),
            TotalSelling = parsed.Selling,
            Currency = "USD",
            HotelCode = parsed.HotelCode,
            HotelName = parsed.HotelName,
            RawJson = $"{{\"demo\":true,\"reference\":\"{reference}\"}}"
        });
    }

    public Task<bool> CancelAsync(string supplierBookingReference, CancellationToken cancellationToken = default) =>
        Task.FromResult(!string.IsNullOrWhiteSpace(supplierBookingReference) &&
                        supplierBookingReference.StartsWith("DEMO-", StringComparison.OrdinalIgnoreCase));

    private static string BuildRateKey(string code, string name, string boardCode, string boardName, decimal selling, HotelSearchRequest request)
    {
        // DEMO|code|name|boardCode|boardName|selling|checkIn|checkOut
        return string.Join('|',
            "DEMO",
            code,
            Uri.EscapeDataString(name),
            boardCode,
            Uri.EscapeDataString(boardName),
            selling.ToString("0.00", CultureInfo.InvariantCulture),
            request.CheckIn.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            request.CheckOut.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
    }

    private static bool TryParseRateKey(string rateKey, out (string HotelCode, string HotelName, string BoardName, decimal Selling, DateTime CheckIn, DateTime CheckOut) parsed)
    {
        parsed = default;
        if (string.IsNullOrWhiteSpace(rateKey) || !rateKey.StartsWith("DEMO|", StringComparison.Ordinal))
            return false;

        var parts = rateKey.Split('|');
        if (parts.Length < 8)
            return false;

        if (!decimal.TryParse(parts[5], NumberStyles.Any, CultureInfo.InvariantCulture, out var selling))
            return false;
        if (!DateTime.TryParseExact(parts[6], "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var checkIn))
            return false;
        if (!DateTime.TryParseExact(parts[7], "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var checkOut))
            return false;

        parsed = (
            parts[1],
            Uri.UnescapeDataString(parts[2]),
            Uri.UnescapeDataString(parts[4]),
            selling,
            checkIn,
            checkOut);
        return true;
    }
}
