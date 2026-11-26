namespace TourismHajj.Helpers;

public class AmadeusOptions
{
    public const string SectionName = "Amadeus";

    /// <summary>Self-Service API Key (Client Id) from developers.amadeus.com.</summary>
    public string ClientId { get; set; } = string.Empty;

    /// <summary>Self-Service API Secret (Client Secret).</summary>
    public string ClientSecret { get; set; } = string.Empty;

    /// <summary>Test: https://test.api.amadeus.com — Production: https://api.amadeus.com</summary>
    public string BaseUrl { get; set; } = "https://test.api.amadeus.com";

    public string DefaultOrigin { get; set; } = "BAH";
    public string DefaultDestination { get; set; } = "JED";
    public int MaxOffers { get; set; } = 20;

    public bool IsConfigured =>
        !string.IsNullOrWhiteSpace(ClientId) &&
        !string.IsNullOrWhiteSpace(ClientSecret) &&
        !ClientId.Contains("YOUR_", StringComparison.OrdinalIgnoreCase);
}
