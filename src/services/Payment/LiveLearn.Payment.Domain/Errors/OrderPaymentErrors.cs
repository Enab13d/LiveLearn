using LiveLearn.BuildingBlocks;

namespace LiveLearn.Payment.Domain.Errors;


public static class OrderPaymentErrors
{
    private static readonly string _prefix = nameof(OrderPaymentErrors);

    public static Error NotFound => Error.NotFound($"{_prefix}.NotFound", "Payment was not found");

    public static Error LineNotFound => Error.NotFound($"{_prefix}.LineNotFound", "Order line was not found in the payment");

    public static Error NotAwaitingPayment => Error.Conflict($"{_prefix}.NotAwaitingPayment", "The payment is already paid, expired or rejected");

    public static Error NoLines => Error.Unprocessable($"{_prefix}.NoLines", "A payment needs at least one order line");

    public static Error NonPositivePrice => Error.Unprocessable($"{_prefix}.NonPositivePrice", "Every order line price must be greater than zero");
}
