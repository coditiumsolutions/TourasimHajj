using TourismHajj.Models.Entities;
using TourismHajj.ViewModels;

namespace TourismHajj.Interfaces;

public interface IFlightSearchService
{
    bool IsLiveSearchAvailable { get; }

    Task<FlightSearchPageViewModel> SearchPageAsync(FlightSearchRequest? request, bool searched, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<FlightOfferItem>> SuggestForPackageAsync(string? packageType, DateTime? departureDate, CancellationToken cancellationToken = default);
    Task<FlightBookPageViewModel?> PrepareBookAsync(string offerId, CancellationToken cancellationToken = default);
    Task<(bool Ok, FlightReservation? Reservation, string? Error)> BookAsync(FlightBookPageViewModel model, int? customerId, CancellationToken cancellationToken = default);
    Task<List<FlightReservationListItem>> GetReservationsAsync(string? q, CancellationToken cancellationToken = default);
    Task<FlightReservation?> GetReservationAsync(int id, CancellationToken cancellationToken = default);
}
