using LiveLearn.BuildingBlocks;
using LiveLearn.Payment.Application.Dto;

namespace LiveLearn.Payment.Application.Queries;


// Polled by the client after placing the order; own payment only, another student's returns not found (D16).
public sealed record GetCheckoutQuery(Guid OrderId) : IQuery<CheckoutDto>;
