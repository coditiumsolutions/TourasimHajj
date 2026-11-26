using Microsoft.Extensions.Options;
using TourismHajj.Helpers;
using TourismHajj.Interfaces;
using TourismHajj.ViewModels;

namespace TourismHajj.Services.Hotelbeds;

/// <summary>
/// Uses Hotelbeds when API keys are present; otherwise falls back to demo inventory for the public Hotels UI.
/// </summary>
public class CompositeHotelProvider : IHotelProvider
{
    private readonly HotelbedsProvider _live;
    private readonly DemoHotelProvider _demo;
    private readonly HotelbedsOptions _options;

    public CompositeHotelProvider(
        HotelbedsProvider live,
        DemoHotelProvider demo,
        IOptions<HotelbedsOptions> options)
    {
        _live = live;
        _demo = demo;
        _options = options.Value;
    }

    private IHotelProvider Active => _options.IsConfigured ? _live : _demo;

    public string ProviderName => Active.ProviderName;

    /// <summary>True when either live Hotelbeds or demo fallback can return search results.</summary>
    public bool IsConfigured => true;

    public bool IsLiveHotelbeds => _options.IsConfigured;

    public Task<IReadOnlyList<HotelSearchResultItem>> SearchAsync(HotelSearchRequest request, CancellationToken cancellationToken = default) =>
        Active.SearchAsync(request, cancellationToken);

    public Task<HotelRateCheckResult?> CheckRateAsync(string rateKey, CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrWhiteSpace(rateKey) && rateKey.StartsWith("DEMO|", StringComparison.Ordinal))
            return _demo.CheckRateAsync(rateKey, cancellationToken);
        return _live.CheckRateAsync(rateKey, cancellationToken);
    }

    public Task<HotelProviderBookingResult> BookAsync(HotelBookRequest request, CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrWhiteSpace(request.RateKey) && request.RateKey.StartsWith("DEMO|", StringComparison.Ordinal))
            return _demo.BookAsync(request, cancellationToken);
        return _live.BookAsync(request, cancellationToken);
    }

    public Task<bool> CancelAsync(string supplierBookingReference, CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrWhiteSpace(supplierBookingReference) &&
            supplierBookingReference.StartsWith("DEMO-", StringComparison.OrdinalIgnoreCase))
            return _demo.CancelAsync(supplierBookingReference, cancellationToken);
        return _live.CancelAsync(supplierBookingReference, cancellationToken);
    }
}
