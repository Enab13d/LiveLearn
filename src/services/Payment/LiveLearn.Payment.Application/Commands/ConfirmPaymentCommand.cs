using LiveLearn.BuildingBlocks;

namespace LiveLearn.Payment.Application.Commands;


// Sent by WebhooksController for checkout.session.completed; the session is re-read for OrderId (D14, C1).
public sealed record ConfirmPaymentCommand(string SessionId) : ICommand;
