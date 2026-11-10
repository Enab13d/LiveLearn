using LiveLearn.BuildingBlocks;

namespace LiveLearn.Ordering.Domain.Errors;


public static class OrderErrors
{
    private static readonly string _prefix = nameof(OrderErrors);

    public static Error NotFound => Error.NotFound($"{_prefix}.NotFound", "Order was not found");

    public static Error LineNotFound => Error.NotFound($"{_prefix}.LineNotFound", "Order line was not found");

    public static Error NothingToOrder => Error.Unprocessable($"{_prefix}.NothingToOrder", "No item in the cart is available for purchase");

    public static Error TotalMismatch => Error.Conflict($"{_prefix}.TotalMismatch", "Cart total has changed; review the cart and confirm the new total");
}
