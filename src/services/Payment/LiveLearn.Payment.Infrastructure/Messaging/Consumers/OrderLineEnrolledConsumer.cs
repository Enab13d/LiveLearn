using LiveLearn.Contracts.Ordering;
using LiveLearn.Payment.Application.Commands;
using LiveLearn.Payment.Infrastructure.Contexts;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LiveLearn.Payment.Infrastructure.Messaging.Consumers;


// Ordering enrolled the line's student; the student may now request its refund (D6).
internal sealed class OrderLineEnrolledConsumer(ISender mediator, ILogger<OrderLineEnrolledConsumer> logger) : IConsumer<OrderLineEnrolled>
{
    public async Task Consume(ConsumeContext<OrderLineEnrolled> context)
    {
        var msg = context.Message;

        logger.LogInformation(
            "Processing message {messageName}, data: {@message}",
            nameof(OrderLineEnrolled), msg);

        var result = await mediator.Send(new MarkLineEnrolledCommand(msg.OrderId, msg.OrderLineId), context.CancellationToken);

        if (result.IsSuccess)
        {
            logger.LogInformation(
                "Successfully processed message {messageName}",
                nameof(OrderLineEnrolled));
        }
        else
        {
            // A business error (e.g. an unknown payment or line) is acknowledged, not retried.
            logger.LogWarning(
                "Failed to process message {messageName}. Errors: {@errors}",
                nameof(OrderLineEnrolled), result.Errors);
        }
    }
}

internal sealed class OrderLineEnrolledConsumerDefinition : ConsumerDefinition<OrderLineEnrolledConsumer>
{
    protected override void ConfigureConsumer(
        IReceiveEndpointConfigurator endpointConfigurator,
        IConsumerConfigurator<OrderLineEnrolledConsumer> consumerConfigurator,
        IRegistrationContext context)
    {
        // An xmin conflict with a concurrent change to the same payment is retried against fresh state (D9).
        endpointConfigurator.UseMessageRetry(cfg =>
        {
            cfg.Exponential(3, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(2));
        });
        endpointConfigurator.UseEntityFrameworkOutbox<WriteDbContext>(context);
    }
}
