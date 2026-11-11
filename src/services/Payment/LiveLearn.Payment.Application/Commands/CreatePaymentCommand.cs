using LiveLearn.BuildingBlocks;
using LiveLearn.Payment.Domain.Common;

namespace LiveLearn.Payment.Application.Commands;


// Sent by RequestPaymentConsumer; the consumer replies PaymentRequestRejected on failure (D10).
public sealed record CreatePaymentCommand(
    Guid OrderId,
    Guid StudentId,
    IReadOnlyList<OrderPaymentLineRequest> Lines) : ICommand;
