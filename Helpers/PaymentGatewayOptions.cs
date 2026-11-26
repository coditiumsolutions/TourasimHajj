namespace TourismHajj.Helpers;

/// <summary>
/// Bahrain-friendly payment providers. Wire live keys when ready; portal currently records the chosen method
/// and confirms bookings (demo/manual) until gateway checkout is enabled.
/// </summary>
public class PaymentGatewayOptions
{
    public const string SectionName = "PaymentGateway";

    /// <summary>Manual | Stripe | Tap | BenefitPay</summary>
    public string Provider { get; set; } = "Manual";

    public string? StripePublishableKey { get; set; }
    public string? StripeSecretKey { get; set; }

    public string? TapPublicKey { get; set; }
    public string? TapSecretKey { get; set; }

    public string? BenefitPayMerchantId { get; set; }

    public bool IsLiveCheckoutEnabled =>
        Provider is "Stripe" or "Tap" or "BenefitPay" &&
        (!string.IsNullOrWhiteSpace(StripeSecretKey) ||
         !string.IsNullOrWhiteSpace(TapSecretKey) ||
         !string.IsNullOrWhiteSpace(BenefitPayMerchantId)) &&
        !(StripeSecretKey?.Contains("YOUR_", StringComparison.OrdinalIgnoreCase) ?? false) &&
        !(TapSecretKey?.Contains("YOUR_", StringComparison.OrdinalIgnoreCase) ?? false);
}
