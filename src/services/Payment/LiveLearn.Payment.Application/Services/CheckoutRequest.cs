using LiveLearn.Payment.Domain.Common;

namespace LiveLearn.Payment.Application.Services;


// ExpiresAt is derived from the payment, not the clock, so every retry sends identical parameters (D14).
public sealed record CheckoutRequest(Guid OrderId, DateTimeOffset ExpiresAt, IReadOnlyList<CheckoutLineItem> Items);

// One line item per course (D1).
public sealed record CheckoutLineItem(Guid CourseId, string CourseTitle, Money Price);
