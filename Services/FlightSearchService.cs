using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TourismHajj.Data;
using TourismHajj.Helpers;
using TourismHajj.Interfaces;
using TourismHajj.Models.Entities;
using TourismHajj.ViewModels;

namespace TourismHajj.Services;

public class FlightSearchService : IFlightSearchService
{
    private readonly IFlightProvider _provider;
    private readonly ApplicationDbContext _db;
    private readonly AmadeusOptions _amadeus;
    private readonly PaymentGatewayOptions _payments;

    public FlightSearchService(
        IFlightProvider provider,
        ApplicationDbContext db,
        IOptions<AmadeusOptions> amadeus,
        IOptions<PaymentGatewayOptions> payments)
    {
        _provider = provider;
        _db = db;
        _amadeus = amadeus.Value;
        _payments = payments.Value;
    }

    public bool IsLiveSearchAvailable => _amadeus.IsConfigured;

    public async Task<FlightSearchPageViewModel> SearchPageAsync(
        FlightSearchRequest? request,
        bool searched,
        CancellationToken cancellationToken = default)
    {
        request ??= new FlightSearchRequest
        {
            Origin = _amadeus.DefaultOrigin,
            Destination = _amadeus.DefaultDestination
        };
        NormalizeSearch(request);

        var isDemo = !_amadeus.IsConfigured;
        var page = new FlightSearchPageViewModel
        {
            Search = request,
            ApiConfigured = true,
            IsDemoMode = isDemo,
            Searched = searched,
            PaymentProvider = _payments.Provider
        };

        if (!searched)
            return page;

        if (request.DepartureDate.Date < DateTime.Today)
        {
            page.ErrorMessage = "Departure date cannot be in the past.";
            return page;
        }

        try
        {
            page.Results = await _provider.SearchAsync(request, cancellationToken);
            if (page.Results.Count == 0)
                page.ErrorMessage = "No flights found for that route/date. Try different dates.";
            else if (isDemo)
                page.ErrorMessage = "Showing sample Bahrain–Saudi flights (demo). Add Amadeus ClientId/ClientSecret for live offers.";
        }
        catch (Exception ex)
        {
            page.ErrorMessage = "Flight search failed: " + ex.Message;
        }

        return page;
    }

    public async Task<IReadOnlyList<FlightOfferItem>> SuggestForPackageAsync(
        string? packageType,
        DateTime? departureDate,
        CancellationToken cancellationToken = default)
    {
        var dest = packageType is "Umrah" or "Hajj" or "Ziarat" ? "JED" : _amadeus.DefaultDestination;
        var request = new FlightSearchRequest
        {
            Origin = _amadeus.DefaultOrigin,
            Destination = dest,
            DepartureDate = departureDate?.Date ?? DateTime.Today.AddDays(21),
            ReturnDate = (departureDate?.Date ?? DateTime.Today.AddDays(21)).AddDays(7),
            Adults = 1
        };
        NormalizeSearch(request);
        return await _provider.SearchAsync(request, cancellationToken);
    }

    public async Task<FlightBookPageViewModel?> PrepareBookAsync(string offerId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(offerId))
            return null;

        var offer = await _provider.GetOfferAsync(offerId, cancellationToken);
        if (offer is null)
            return null;

        return new FlightBookPageViewModel
        {
            OfferId = offer.OfferId,
            CarrierName = offer.CarrierName,
            FlightNumber = offer.FlightNumber,
            Origin = offer.Origin,
            Destination = offer.Destination,
            DepartureAt = offer.DepartureAt,
            ArrivalAt = offer.ArrivalAt,
            TotalPrice = offer.TotalPrice,
            Currency = offer.Currency,
            Summary = offer.Summary,
            IsRoundTrip = offer.IsRoundTrip,
            PaymentMethod = _payments.Provider,
            AvailablePaymentMethods = AvailablePaymentMethods()
        };
    }

    public async Task<(bool Ok, FlightReservation? Reservation, string? Error)> BookAsync(
        FlightBookPageViewModel model,
        int? customerId,
        CancellationToken cancellationToken = default)
    {
        var offer = await _provider.GetOfferAsync(model.OfferId, cancellationToken);
        if (offer is null)
            return (false, null, "This flight offer expired. Please search again.");

        var clientRef = "THF" + DateTime.UtcNow.ToString("yyMMddHHmmss");
        var result = await _provider.BookAsync(new FlightBookRequest
        {
            OfferId = offer.OfferId,
            TravelerFirstName = model.TravelerFirstName.Trim(),
            TravelerLastName = model.TravelerLastName.Trim(),
            Email = model.Email?.Trim(),
            Phone = model.Phone?.Trim(),
            ClientReference = clientRef
        }, cancellationToken);

        var paymentMethod = string.IsNullOrWhiteSpace(model.PaymentMethod)
            ? _payments.Provider
            : model.PaymentMethod.Trim();

        var reservation = new FlightReservation
        {
            Provider = _provider.ProviderName,
            SupplierBookingReference = result.SupplierBookingReference,
            ClientReference = clientRef,
            TicketNumber = result.TicketNumber,
            Origin = offer.Origin,
            Destination = offer.Destination,
            FlightNumber = offer.FlightNumber,
            CarrierName = offer.CarrierName,
            DepartureAt = offer.DepartureAt,
            ArrivalAt = offer.ArrivalAt,
            IsRoundTrip = offer.IsRoundTrip,
            SellingAmount = result.TotalPrice ?? offer.TotalPrice,
            Currency = result.Currency ?? offer.Currency,
            Status = result.Success ? (result.Status ?? "TICKETED") : "FAILED",
            PaymentMethod = paymentMethod,
            PaymentStatus = result.Success
                ? (_payments.IsLiveCheckoutEnabled ? "CAPTURED" : "PENDING_SETTLEMENT")
                : "FAILED",
            TravelerFirstName = model.TravelerFirstName.Trim(),
            TravelerLastName = model.TravelerLastName.Trim(),
            Email = model.Email?.Trim(),
            Phone = model.Phone?.Trim(),
            OfferSummary = offer.Summary,
            SupplierRawJson = result.RawJson,
            CustomerId = customerId,
            CreatedDate = DateTime.UtcNow
        };

        _db.FlightReservations.Add(reservation);
        await _db.SaveChangesAsync(cancellationToken);

        if (!result.Success)
            return (false, reservation, result.ErrorMessage ?? "Flight booking failed.");

        return (true, reservation, null);
    }

    public async Task<List<FlightReservationListItem>> GetReservationsAsync(string? q, CancellationToken cancellationToken = default)
    {
        var query = _db.FlightReservations.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(q))
        {
            query = query.Where(r =>
                r.FlightNumber.Contains(q) ||
                r.Origin.Contains(q) ||
                r.Destination.Contains(q) ||
                (r.SupplierBookingReference != null && r.SupplierBookingReference.Contains(q)) ||
                (r.TicketNumber != null && r.TicketNumber.Contains(q)) ||
                (r.TravelerLastName != null && r.TravelerLastName.Contains(q)) ||
                r.Status.Contains(q));
        }

        return await query
            .OrderByDescending(r => r.CreatedDate)
            .Take(200)
            .Select(r => new FlightReservationListItem
            {
                FlightReservationId = r.FlightReservationId,
                Provider = r.Provider,
                SupplierBookingReference = r.SupplierBookingReference,
                TicketNumber = r.TicketNumber,
                Route = r.Origin + " → " + r.Destination,
                FlightNumber = r.FlightNumber,
                DepartureAt = r.DepartureAt,
                Status = r.Status,
                SellingAmount = r.SellingAmount,
                Currency = r.Currency,
                HolderName = ((r.TravelerFirstName ?? "") + " " + (r.TravelerLastName ?? "")).Trim(),
                PaymentMethod = r.PaymentMethod,
                CreatedDate = r.CreatedDate
            })
            .ToListAsync(cancellationToken);
    }

    public Task<FlightReservation?> GetReservationAsync(int id, CancellationToken cancellationToken = default) =>
        _db.FlightReservations.AsNoTracking().FirstOrDefaultAsync(r => r.FlightReservationId == id, cancellationToken);

    private IReadOnlyList<string> AvailablePaymentMethods()
    {
        var list = new List<string> { "Manual", "BankTransfer" };
        if (!string.IsNullOrWhiteSpace(_payments.Provider) &&
            !list.Contains(_payments.Provider, StringComparer.OrdinalIgnoreCase))
            list.Insert(0, _payments.Provider);
        foreach (var p in new[] { "Tap", "Stripe", "BenefitPay" })
        {
            if (!list.Contains(p, StringComparer.OrdinalIgnoreCase))
                list.Add(p);
        }
        return list;
    }

    private void NormalizeSearch(FlightSearchRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Origin))
            request.Origin = _amadeus.DefaultOrigin;
        if (string.IsNullOrWhiteSpace(request.Destination))
            request.Destination = _amadeus.DefaultDestination;
        request.Origin = request.Origin.Trim().ToUpperInvariant();
        request.Destination = request.Destination.Trim().ToUpperInvariant();
        if (request.DepartureDate.Date < DateTime.Today)
            request.DepartureDate = DateTime.Today.AddDays(14);
        if (!request.OneWay)
        {
            if (!request.ReturnDate.HasValue || request.ReturnDate.Value.Date <= request.DepartureDate.Date)
                request.ReturnDate = request.DepartureDate.AddDays(7);
        }
        if (request.Adults < 1) request.Adults = 1;
        if (request.Children < 0) request.Children = 0;
    }
}
