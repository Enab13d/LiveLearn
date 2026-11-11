namespace LiveLearn.Payment.Domain.Enums;

// Paid → RefundRequested → Refunded; RefundFailed is final (D13). AwaitingPayment until the payment is paid,
// and for good if it expires or is rejected.
public enum OrderPaymentLineStatus
{
    AwaitingPayment,
    Paid,
    RefundRequested,
    Refunded,
    // Stripe couldn't return the money; reached from RefundRequested or Refunded, final (D6).
    RefundFailed
}
