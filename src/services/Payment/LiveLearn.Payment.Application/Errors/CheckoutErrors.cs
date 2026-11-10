using LiveLearn.BuildingBlocks;

namespace LiveLearn.Payment.Application.Errors;


public static class CheckoutErrors
{
    public static Error Rejected(string reason) =>
        Error.Unprocessable("CheckoutErrors.Rejected", $"The payment provider rejected the checkout session: {reason}");

    public static Error UnknownSession(string sessionId) =>
        Error.NotFound("CheckoutErrors.UnknownSession", $"Checkout session '{sessionId}' doesn't exist or carries no OrderId.");
}
