namespace LiveLearn.Payment.Application.Configuration;

// Revenue split (D5). Defaults apply when the "RevenueShare" section is absent.
public sealed class RevenueShareOptions
{
    public static string SectionName => "RevenueShare";

    // The tutor's share of a line's price; snapshotted per line at creation, so a change affects new sales only.
    public decimal TutorShareRate { get; set; } = 0.70m;
}
