namespace LiveLearn.Payment.Domain.Enums;

public enum OrderPaymentStatus
{
    AwaitingPayment,
    Paid,
    Expired,
    // Stripe refused the Checkout Session itself; terminal (D14).
    Rejected
}
