using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TourismHajj.Data;
using TourismHajj.Helpers;
using TourismHajj.Interfaces;
using TourismHajj.Models.Entities;
using TourismHajj.ViewModels;

namespace TourismHajj.Services;

public class HotelSearchService : IHotelSearchService
{
    private readonly IHotelProvider _provider;
    private readonly IPublicSiteService _publicSite;
    private readonly ApplicationDbContext _db;
    private readonly HotelbedsOptions _hotelbeds;

    public HotelSearchService(
        IHotelProvider provider,
        IPublicSiteService publicSite,
        ApplicationDbContext db,
        IOptions<HotelbedsOptions> hotelbeds)
    {
        _provider = provider;
        _publicSite = publicSite;
        _db = db;
        _hotelbeds = hotelbeds.Value;
    }

    public bool IsLiveSearchAvailable => _hotelbeds.IsConfigured;

    public async Task<HotelSearchPageViewModel> SearchPageAsync(HotelSearchRequest? request, bool searched, CancellationToken cancellationToken = default)
    {
        request ??= new HotelSearchRequest();
        NormalizeSearch(request);

        var isDemo = !_hotelbeds.IsConfigured;
        var page = new HotelSearchPageViewModel
        {
            Search = request,
            ApiConfigured = true,
            IsDemoMode = isDemo,
            Searched = searched,
            ManualPackages = await _publicSite.GetPublicHotelsAsync(cancellationToken)
        };

        if (!searched)
            return page;

        if (request.CheckOut <= request.CheckIn)
        {
            page.ErrorMessage = "Check-out must be after check-in.";
            return page;
        }

        try
        {
            page.LiveResults = await _provider.SearchAsync(request, cancellationToken);
            if (page.LiveResults.Count == 0)
            {
                page.ErrorMessage = "No availability found for those dates. Try different dates or browse curated packages below.";
            }
            else if (isDemo)
            {
                page.ErrorMessage = "Showing sample Makkah/Madinah rates (demo). Add Hotelbeds ApiKey/Secret in appsettings to switch to live inventory.";
            }
        }
        catch (Exception ex)
        {
            page.ErrorMessage = "Hotel search failed: " + ex.Message;
        }

        return page;
    }

    public async Task<HotelDetailPageViewModel?> GetDetailAsync(string hotelCode, HotelSearchRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(hotelCode))
            return null;

        NormalizeSearch(request);
        var results = await _provider.SearchAsync(request, cancellationToken);
        var hotel = results.FirstOrDefault(h => string.Equals(h.HotelCode, hotelCode, StringComparison.OrdinalIgnoreCase));
        if (hotel is null)
            return null;

        return new HotelDetailPageViewModel
        {
            Search = request,
            Hotel = hotel
        };
    }

    public async Task<HotelBookPageViewModel?> PrepareBookAsync(string rateKey, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(rateKey))
            return null;

        var checkedRate = await _provider.CheckRateAsync(rateKey, cancellationToken);
        if (checkedRate is null)
            return null;

        return new HotelBookPageViewModel
        {
            RateKey = checkedRate.RateKey,
            HotelCode = checkedRate.HotelCode,
            HotelName = checkedRate.HotelName,
            Destination = InferDestination(checkedRate.HotelName),
            RoomName = checkedRate.RoomName,
            BoardName = checkedRate.BoardName,
            SellingRate = checkedRate.SellingRate,
            Currency = checkedRate.Currency,
            CancellationPolicySummary = checkedRate.CancellationPolicySummary,
            CheckIn = checkedRate.CheckIn,
            CheckOut = checkedRate.CheckOut
        };
    }

    public async Task<(bool Ok, HotelReservation? Reservation, string? Error)> BookAsync(
        HotelBookPageViewModel model,
        int? customerId,
        CancellationToken cancellationToken = default)
    {
        var checkedRate = await _provider.CheckRateAsync(model.RateKey, cancellationToken);
        if (checkedRate is null)
            return (false, null, "This rate is no longer available. Please search again.");

        var guestFirst = string.IsNullOrWhiteSpace(model.GuestFirstName) ? model.HolderFirstName : model.GuestFirstName;
        var guestLast = string.IsNullOrWhiteSpace(model.GuestLastName) ? model.HolderLastName : model.GuestLastName;
        var clientRef = "TH" + DateTime.UtcNow.ToString("yyMMddHHmmss");

        var bookRequest = new HotelBookRequest
        {
            RateKey = checkedRate.RateKey,
            HolderFirstName = model.HolderFirstName.Trim(),
            HolderLastName = model.HolderLastName.Trim(),
            HolderEmail = model.HolderEmail?.Trim(),
            HolderPhone = model.HolderPhone?.Trim(),
            ClientReference = clientRef,
            Guests =
            {
                new HotelBookPax
                {
                    Type = "AD",
                    FirstName = guestFirst.Trim(),
                    LastName = guestLast.Trim()
                }
            }
        };

        var result = await _provider.BookAsync(bookRequest, cancellationToken);
        var reservation = new HotelReservation
        {
            Provider = _provider.ProviderName,
            SupplierBookingReference = result.SupplierBookingReference,
            ClientReference = clientRef,
            HotelCode = result.HotelCode ?? checkedRate.HotelCode,
            HotelName = result.HotelName ?? checkedRate.HotelName,
            Destination = string.IsNullOrWhiteSpace(model.Destination)
                ? InferDestination(checkedRate.HotelName)
                : model.Destination.Trim(),
            RoomName = checkedRate.RoomName,
            BoardName = checkedRate.BoardName,
            CheckIn = checkedRate.CheckIn,
            CheckOut = checkedRate.CheckOut,
            Rooms = 1,
            Adults = 2,
            Children = 0,
            NetAmount = result.TotalNet ?? checkedRate.Net,
            SellingAmount = result.TotalSelling > 0 ? result.TotalSelling : checkedRate.SellingRate,
            Currency = result.Currency ?? checkedRate.Currency,
            Status = result.Success ? (result.Status ?? "CONFIRMED") : "FAILED",
            HolderFirstName = model.HolderFirstName.Trim(),
            HolderLastName = model.HolderLastName.Trim(),
            HolderEmail = model.HolderEmail?.Trim(),
            HolderPhone = model.HolderPhone?.Trim(),
            GuestsJson = JsonSerializer.Serialize(bookRequest.Guests),
            SupplierRawJson = result.RawJson,
            CustomerId = customerId,
            CreatedDate = DateTime.UtcNow
        };

        _db.HotelReservations.Add(reservation);
        await _db.SaveChangesAsync(cancellationToken);

        if (!result.Success)
            return (false, reservation, result.ErrorMessage ?? "Booking failed with supplier.");

        return (true, reservation, null);
    }

    public async Task<List<HotelReservationListItem>> GetReservationsAsync(string? q, CancellationToken cancellationToken = default)
    {
        var query = _db.HotelReservations.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(q))
        {
            query = query.Where(r =>
                r.HotelName.Contains(q) ||
                (r.SupplierBookingReference != null && r.SupplierBookingReference.Contains(q)) ||
                (r.HolderEmail != null && r.HolderEmail.Contains(q)) ||
                (r.HolderLastName != null && r.HolderLastName.Contains(q)) ||
                r.Status.Contains(q));
        }

        return await query
            .OrderByDescending(r => r.CreatedDate)
            .Take(200)
            .Select(r => new HotelReservationListItem
            {
                HotelReservationId = r.HotelReservationId,
                Provider = r.Provider,
                SupplierBookingReference = r.SupplierBookingReference,
                HotelName = r.HotelName,
                Destination = r.Destination,
                CheckIn = r.CheckIn,
                CheckOut = r.CheckOut,
                Status = r.Status,
                SellingAmount = r.SellingAmount,
                Currency = r.Currency,
                HolderName = ((r.HolderFirstName ?? "") + " " + (r.HolderLastName ?? "")).Trim(),
                CreatedDate = r.CreatedDate
            })
            .ToListAsync(cancellationToken);
    }

    public Task<HotelReservation?> GetReservationAsync(int id, CancellationToken cancellationToken = default) =>
        _db.HotelReservations.AsNoTracking().FirstOrDefaultAsync(r => r.HotelReservationId == id, cancellationToken);

    private static void NormalizeSearch(HotelSearchRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Destination))
            request.Destination = "Makkah";
        if (request.CheckIn.Date < DateTime.Today)
            request.CheckIn = DateTime.Today.AddDays(7);
        if (request.CheckOut.Date <= request.CheckIn.Date)
            request.CheckOut = request.CheckIn.AddDays(3);
        if (request.Rooms < 1) request.Rooms = 1;
        if (request.Adults < 1) request.Adults = 2;
        if (request.Children < 0) request.Children = 0;
    }

    private static string InferDestination(string hotelName) =>
        hotelName.Contains("Madin", StringComparison.OrdinalIgnoreCase) ||
        hotelName.Contains("Medin", StringComparison.OrdinalIgnoreCase)
            ? "Madinah"
            : "Makkah";
}
