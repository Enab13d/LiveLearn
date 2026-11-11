namespace LiveLearn.Payment.Application.Messages;

// Payment-internal: creates the Stripe Checkout Session after the payment is committed, never inside its
// transaction (D14). Published through the outbox together with the new payment.
public sealed record CreateCheckoutSession(Guid OrderId);
