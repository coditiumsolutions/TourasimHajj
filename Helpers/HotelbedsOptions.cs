namespace TourismHajj.Helpers;

public class HotelbedsOptions
{
    public const string SectionName = "Hotelbeds";

    /// <summary>API key from developer.hotelbeds.com (leave empty to disable live search).</summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>API secret from developer.hotelbeds.com.</summary>
    public string Secret { get; set; } = string.Empty;

    /// <summary>Sandbox: https://api.test.hotelbeds.com — Live: https://api.hotelbeds.com</summary>
    public string BaseUrl { get; set; } = "https://api.test.hotelbeds.com";

    public string MakkahDestinationCode { get; set; } = "MKK";
    public string MadinahDestinationCode { get; set; } = "MED";

    public int MaxHotels { get; set; } = 40;

    public bool IsConfigured =>
        !string.IsNullOrWhiteSpace(ApiKey) &&
        !string.IsNullOrWhiteSpace(Secret) &&
        !ApiKey.Contains("YOUR_", StringComparison.OrdinalIgnoreCase);
}
