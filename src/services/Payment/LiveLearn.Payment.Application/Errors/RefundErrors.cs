using LiveLearn.BuildingBlocks;

namespace LiveLearn.Payment.Application.Errors;


public static class RefundErrors
{
    public static Error Rejected(string reason) =>
        Error.Unprocessable("RefundErrors.Rejected", $"The payment provider rejected the refund: {reason}");

    public static Error UnknownRefund(string refundId) =>
        Error.NotFound("RefundErrors.UnknownRefund", $"Refund '{refundId}' doesn't exist or carries no OrderId / OrderLineId.");
}
