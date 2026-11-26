using TourismHajj.ViewModels;

namespace TourismHajj.Interfaces;

public interface IHotelProvider
{
    string ProviderName { get; }
    bool IsConfigured { get; }

    Task<IReadOnlyList<HotelSearchResultItem>> SearchAsync(HotelSearchRequest request, CancellationToken cancellationToken = default);
    Task<HotelRateCheckResult?> CheckRateAsync(string rateKey, CancellationToken cancellationToken = default);
    Task<HotelProviderBookingResult> BookAsync(HotelBookRequest request, CancellationToken cancellationToken = default);
    Task<bool> CancelAsync(string supplierBookingReference, CancellationToken cancellationToken = default);
}
