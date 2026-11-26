using Microsoft.Extensions.Options;
using TourismHajj.Helpers;
using TourismHajj.Interfaces;
using TourismHajj.ViewModels;

namespace TourismHajj.Services.Amadeus;

public class CompositeFlightProvider : IFlightProvider
{
    private readonly AmadeusProvider _live;
    private readonly DemoFlightProvider _demo;
    private readonly AmadeusOptions _options;

    public CompositeFlightProvider(
        AmadeusProvider live,
        DemoFlightProvider demo,
        IOptions<AmadeusOptions> options)
    {
        _live = live;
        _demo = demo;
        _options = options.Value;
    }

    private IFlightProvider Active => _options.IsConfigured ? _live : _demo;

    public string ProviderName => Active.ProviderName;
    public bool IsConfigured => true;
    public bool IsLiveAmadeus => _options.IsConfigured;

    public Task<IReadOnlyList<FlightOfferItem>> SearchAsync(FlightSearchRequest request, CancellationToken cancellationToken = default) =>
        Active.SearchAsync(request, cancellationToken);

    public Task<FlightOfferItem?> GetOfferAsync(string offerId, CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrWhiteSpace(offerId) && offerId.StartsWith("DEMO|", StringComparison.Ordinal))
            return _demo.GetOfferAsync(offerId, cancellationToken);
        return _live.GetOfferAsync(offerId, cancellationToken);
    }

    public Task<FlightProviderBookingResult> BookAsync(FlightBookRequest request, CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrWhiteSpace(request.OfferId) && request.OfferId.StartsWith("DEMO|", StringComparison.Ordinal))
            return _demo.BookAsync(request, cancellationToken);
        return _live.BookAsync(request, cancellationToken);
    }
}
