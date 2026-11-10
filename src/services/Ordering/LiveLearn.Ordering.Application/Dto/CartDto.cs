namespace LiveLearn.Ordering.Application.Dto;


// Total = Σ CurrentPrice of the available items — what POST /orders expects as expectedTotal (O5).
public sealed record CartDto(IReadOnlyList<CartItemDto> Items, decimal Total);
