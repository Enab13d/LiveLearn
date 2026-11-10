namespace LiveLearn.Ordering.Application.Configuration;

// Cart limits (O3). Defaults apply when the "Cart" section is absent.
public sealed class CartOptions
{
    public static string SectionName => "Cart";

    public int MaxItems { get; set; } = 20;

    // Sliding: refreshed on every change to the cart.
    public TimeSpan Ttl { get; set; } = TimeSpan.FromDays(30);
}
