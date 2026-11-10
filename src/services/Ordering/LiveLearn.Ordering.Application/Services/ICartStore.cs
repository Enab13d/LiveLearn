namespace LiveLearn.Ordering.Application.Services;

// A student's cart (O3). Losing a cart is acceptable, so callers may treat a store failure as an empty cart.
public interface ICartStore
{
    Task<IReadOnlyList<CartItem>> GetAsync(Guid studentId, CancellationToken ct);

    // Checks "already in the cart" and the CartOptions.MaxItems limit atomically with the write.
    Task<CartAddResult> AddAsync(Guid studentId, CartItem item, CancellationToken ct);

    Task RemoveAsync(Guid studentId, IReadOnlyCollection<Guid> courseIds, CancellationToken ct);

    Task ClearAsync(Guid studentId, CancellationToken ct);
}
