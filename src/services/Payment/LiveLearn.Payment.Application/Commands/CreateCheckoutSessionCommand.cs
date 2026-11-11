using LiveLearn.BuildingBlocks;

namespace LiveLearn.Payment.Application.Commands;


// Sent by CreateCheckoutSessionConsumer once the payment is committed (D14).
public sealed record CreateCheckoutSessionCommand(Guid OrderId) : ICommand;
