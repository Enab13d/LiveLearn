namespace LiveLearn.Payment.Domain.Common;

// One order line as requested by Ordering (RequestPayment); price and currency are still unchecked.
public sealed record OrderPaymentLineRequest(Guid OrderLineId, Guid CourseId, Guid TutorId, decimal Price, string Currency);
