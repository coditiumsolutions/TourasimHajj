using TourismHajj.Models.Entities;
using TourismHajj.ViewModels;

namespace TourismHajj.Interfaces;

public interface IHotelSearchService
{
    bool IsLiveSearchAvailable { get; }

    Task<HotelSearchPageViewModel> SearchPageAsync(HotelSearchRequest? request, bool searched, CancellationToken cancellationToken = default);
    Task<HotelDetailPageViewModel?> GetDetailAsync(string hotelCode, HotelSearchRequest request, CancellationToken cancellationToken = default);
    Task<HotelBookPageViewModel?> PrepareBookAsync(string rateKey, CancellationToken cancellationToken = default);
    Task<(bool Ok, HotelReservation? Reservation, string? Error)> BookAsync(HotelBookPageViewModel model, int? customerId, CancellationToken cancellationToken = default);
    Task<List<HotelReservationListItem>> GetReservationsAsync(string? q, CancellationToken cancellationToken = default);
    Task<HotelReservation?> GetReservationAsync(int id, CancellationToken cancellationToken = default);
}
