namespace LiveLearn.Ordering.Infrastructure.Models;


internal sealed class OrderReadModel
{
    public Guid Id { get; set; }
    public Guid StudentId { get; set; }
    public DateTimeOffset PlacedAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public List<OrderLineReadModel> Lines { get; set; } = [];
}
