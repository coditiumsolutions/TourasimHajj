using TourismHajj.ViewModels;

namespace TourismHajj.Interfaces;

public interface IFlightProvider
{
    string ProviderName { get; }
    bool IsConfigured { get; }

    Task<IReadOnlyList<FlightOfferItem>> SearchAsync(FlightSearchRequest request, CancellationToken cancellationToken = default);
    Task<FlightOfferItem?> GetOfferAsync(string offerId, CancellationToken cancellationToken = default);
    Task<FlightProviderBookingResult> BookAsync(FlightBookRequest request, CancellationToken cancellationToken = default);
}
