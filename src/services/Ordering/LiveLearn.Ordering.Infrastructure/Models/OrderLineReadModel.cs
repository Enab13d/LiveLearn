namespace LiveLearn.Ordering.Infrastructure.Models;


internal sealed class OrderLineReadModel
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public Guid CourseId { get; set; }
    public Guid TutorId { get; set; }
    public decimal Price { get; set; }
    public string Status { get; set; } = string.Empty;
}
