namespace LiveLearn.Ordering.Application.Services;

// PriceAtAdd only detects price changes; the charged price always comes from the projection at placement (O3).
public sealed record CartItem(Guid CourseId, DateTimeOffset AddedAt, decimal PriceAtAdd);
