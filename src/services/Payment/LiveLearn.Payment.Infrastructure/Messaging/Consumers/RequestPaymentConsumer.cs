using LiveLearn.Contracts.Payment;
using LiveLearn.Payment.Application.Commands;
using LiveLearn.Payment.Domain.Common;
using LiveLearn.Payment.Infrastructure.Contexts;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LiveLearn.Payment.Infrastructure.Messaging.Consumers;


// One step of the purchase saga: a rejected request is replied to the saga; success is replied later by
// the webhook path (PaymentSucceeded / PaymentExpired).
internal sealed class RequestPaymentConsumer(ISender mediator, TimeProvider timeProvider, ILogger<RequestPaymentConsumer> logger)
    : IConsumer<RequestPayment>
{
    public async Task Consume(ConsumeContext<RequestPayment> context)
    {
        var msg = context.Message;

        logger.LogInformation(
            "Processing message {messageName}, data: {@message}",
            nameof(RequestPayment), msg);

        var lines = msg.Lines
            .Select(l => new OrderPaymentLineRequest(l.OrderLineId, l.CourseId, l.TutorId, l.CourseTitle, l.Price, l.Currency))
            .ToList();

        var result = await mediator.Send(new CreatePaymentCommand(msg.OrderId, msg.StudentId, lines), context.CancellationToken);

        if (result.IsSuccess)
        {
            logger.LogInformation(
                "Successfully processed message {messageName}",
                nameof(RequestPayment));
            return;
        }

        // A business outcome (e.g. a zero-price line), not a fault: acknowledged; the saga cancels the order.
        await context.Publish(new PaymentRequestRejected(
            msg.OrderId, result.FirstError.Message, timeProvider.GetUtcNow()), context.CancellationToken);

        logger.LogWarning(
            "Failed to process message {messageName}. Errors: {@errors}",
            nameof(RequestPayment), result.Errors);
    }
}

// Endpoint "payment-request-payment": the Ordering saga sends RequestPayment there.
internal sealed class RequestPaymentConsumerDefinition : ConsumerDefinition<RequestPaymentConsumer>
{
    protected override void ConfigureConsumer(
        IReceiveEndpointConfigurator endpointConfigurator,
        IConsumerConfigurator<RequestPaymentConsumer> consumerConfigurator,
        IRegistrationContext context)
    {
        // a concurrent duplicate on the unique OrderId is retried into the "already exists" success
        endpointConfigurator.UseMessageRetry(cfg => cfg.Interval(3, TimeSpan.FromSeconds(1)));
        endpointConfigurator.UseEntityFrameworkOutbox<WriteDbContext>(context);
    }
}
