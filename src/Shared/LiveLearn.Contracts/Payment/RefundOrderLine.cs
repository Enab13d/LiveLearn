namespace LiveLearn.Contracts.Payment;

// Command sent by the Ordering saga to Payment's queue to refund one line as compensation (e.g. its
// enrollment failed); no enrollment needed, otherwise the student's refund rules apply (D6).
public sealed record RefundOrderLine(
    Guid OrderId,
    Guid OrderLineId,
    string Reason
);
