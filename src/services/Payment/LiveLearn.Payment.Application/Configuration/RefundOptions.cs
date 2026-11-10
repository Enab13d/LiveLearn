namespace LiveLearn.Payment.Application.Configuration;

// Refund window (D6). Defaults apply when the "Refund" section is absent.
public sealed class RefundOptions
{
    public static string SectionName => "Refund";

    // Counted from PaidAt; snapshotted on the payment as RefundWindowEndsAt.
    public int WindowDays { get; set; } = 14;
}
