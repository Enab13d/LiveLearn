namespace LiveLearn.Catalog.Infrastructure.Configuration;


// Streaming validates tickets with the matching public key; it never sees PrivateKey.
internal sealed class WatchTicketOptions
{
    public static string SectionName => "WatchTicket";

    // Base64-encoded PKCS#8 ECDSA P-256 private key.
    public string PrivateKey { get; set; } = string.Empty;

    public string Issuer { get; set; } = "livelearn-catalog";

    public string Audience { get; set; } = "livelearn-streaming";

    public int TtlMinutes { get; set; } = 3;
}
