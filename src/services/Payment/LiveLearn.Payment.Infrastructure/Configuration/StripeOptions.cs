namespace LiveLearn.Payment.Infrastructure.Configuration;

// No API version here: Stripe.net pins it per package version (StripeConfiguration.ApiVersion); webhook
// destinations must be created with that same version.
internal sealed class StripeOptions
{
    public static string SectionName => "Stripe";

    public string SecretKey { get; set; } = string.Empty;

    // One signing secret per webhook destination (D15).
    public string PlatformWebhookSecret { get; set; } = string.Empty;
    public string ConnectWebhookSecret { get; set; } = string.Empty;
    public string ThinWebhookSecret { get; set; } = string.Empty;

    // SPA pages Stripe Checkout redirects the browser to.
    public string CheckoutSuccessUrl { get; set; } = string.Empty;
    public string CheckoutCancelUrl { get; set; } = string.Empty;
}
