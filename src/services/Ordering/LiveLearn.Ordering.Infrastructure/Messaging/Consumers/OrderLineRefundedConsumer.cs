using LiveLearn.Contracts.Payment;
using LiveLearn.Ordering.Application.Commands;
using LiveLearn.Ordering.Infrastructure.Contexts;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LiveLearn.Ordering.Infrastructure.Messaging.Consumers;


// Payment confirmed the refund of this line; marks it Refunded (O10).
internal sealed class OrderLineRefundedConsumer(ISender mediator, ILogger<OrderLineRefundedConsumer> logger) : IConsumer<OrderLineRefunded>
{
    public async Task Consume(ConsumeContext<OrderLineRefunded> context)
    {
        var msg = context.Message;

        logger.LogInformation(
            "Processing message {messageName}, data: {@message}",
            nameof(OrderLineRefunded), msg);

        var result = await mediator.Send(new RecordLineRefundedCommand(msg.OrderId, msg.OrderLineId), context.CancellationToken);

        if (result.IsSuccess)
        {
            logger.LogInformation(
                "Successfully processed message {messageName}",
                nameof(OrderLineRefunded));
        }
        else
        {
            // A business error (e.g. an unknown order) is acknowledged, not retried.
            logger.LogWarning(
                "Failed to process message {messageName}. Errors: {@errors}",
                nameof(OrderLineRefunded), result.Errors);
        }
    }
}

internal sealed class OrderLineRefundedConsumerDefinition : ConsumerDefinition<OrderLineRefundedConsumer>
{
    protected override void ConfigureConsumer(
        IReceiveEndpointConfigurator endpointConfigurator,
        IConsumerConfigurator<OrderLineRefundedConsumer> consumerConfigurator,
        IRegistrationContext context)
    {
        // An xmin conflict with a concurrent change to the same order is retried against fresh state.
        endpointConfigurator.UseMessageRetry(cfg =>
        {
            cfg.Exponential(3, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(2));
        });
        endpointConfigurator.UseEntityFrameworkOutbox<WriteDbContext>(context);
    }
}
