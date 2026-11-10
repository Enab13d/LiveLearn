namespace LiveLearn.Ordering.Application.Dto;


// Course titles are not here: the frontend merges them from Catalog by CourseId (O6: no title snapshot).
public sealed record OrderDto(
    Guid Id,
    DateTimeOffset PlacedAt,
    string Status,
    decimal Total,
    IReadOnlyList<OrderLineDto> Lines
);
