namespace LiveLearn.Payment.Infrastructure.Models;


internal sealed class OrderPaymentReadModel
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public Guid StudentId { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? CheckoutUrl { get; set; }
}
