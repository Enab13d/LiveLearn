namespace LiveLearn.Payment.Domain.Enums;

public enum OrderPaymentStatus
{
    AwaitingPayment,
    Paid,
    // Derived from the lines: some (PartiallyRefunded) or all (Refunded) of them refunded; a failed
    // refund counts as not refunded, so the status may step back (D13).
    PartiallyRefunded,
    Refunded,
    Expired,
    // Stripe refused the Checkout Session itself; terminal (D14).
    Rejected
}
