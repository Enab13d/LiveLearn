using LiveLearn.Contracts.Payment;
using LiveLearn.Ordering.Application.Commands;
using LiveLearn.Ordering.Infrastructure.Contexts;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LiveLearn.Ordering.Infrastructure.Messaging.Consumers;


// Stripe failed a confirmed refund; marks the line RefundFailed and logs an alert (O6, D6).
internal sealed class OrderLineRefundFailedConsumer(ISender mediator, ILogger<OrderLineRefundFailedConsumer> logger) : IConsumer<OrderLineRefundFailed>
{
    public async Task Consume(ConsumeContext<OrderLineRefundFailed> context)
    {
        var msg = context.Message;

        logger.LogInformation(
            "Processing message {messageName}, data: {@message}",
            nameof(OrderLineRefundFailed), msg);

        var result = await mediator.Send(new RecordLineRefundFailedCommand(msg.OrderId, msg.OrderLineId), context.CancellationToken);

        if (result.IsSuccess)
        {
            logger.LogInformation(
                "Successfully processed message {messageName}",
                nameof(OrderLineRefundFailed));
        }
        else
        {
            // A business error (e.g. an unknown order) is acknowledged, not retried.
            logger.LogWarning(
                "Failed to process message {messageName}. Errors: {@errors}",
                nameof(OrderLineRefundFailed), result.Errors);
        }
    }
}

internal sealed class OrderLineRefundFailedConsumerDefinition : ConsumerDefinition<OrderLineRefundFailedConsumer>
{
    protected override void ConfigureConsumer(
        IReceiveEndpointConfigurator endpointConfigurator,
        IConsumerConfigurator<OrderLineRefundFailedConsumer> consumerConfigurator,
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
