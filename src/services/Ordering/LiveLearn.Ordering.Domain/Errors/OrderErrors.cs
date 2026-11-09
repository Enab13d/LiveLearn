using LiveLearn.BuildingBlocks;

namespace LiveLearn.Ordering.Domain.Errors;


public static class OrderErrors
{
    private static readonly string _prefix = nameof(OrderErrors);

    public static Error NotFound => Error.NotFound($"{_prefix}.NotFound", "Order was not found");

    public static Error LineNotFound => Error.NotFound($"{_prefix}.LineNotFound", "Order line was not found");
}
