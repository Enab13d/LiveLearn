namespace LiveLearn.Ordering.Application.Dto;


// Validated lazily on every read (O4). UnavailableReason / PendingOrderId are null while available;
// PendingOrderId lets the UI offer "Continue payment". Titles come from Catalog, as for orders.
public sealed record CartItemDto(
    Guid CourseId,
    DateTimeOffset AddedAt,
    decimal PriceAtAdd,
    decimal? CurrentPrice,
    bool PriceChanged,
    bool IsAvailable,
    string? UnavailableReason,
    Guid? PendingOrderId
);
