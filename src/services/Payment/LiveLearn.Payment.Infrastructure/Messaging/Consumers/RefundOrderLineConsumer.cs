using LiveLearn.Contracts.Payment;
using LiveLearn.Payment.Application.Commands;
using LiveLearn.Payment.Infrastructure.Contexts;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LiveLearn.Payment.Infrastructure.Messaging.Consumers;


// The saga's compensation for a line (e.g. its enrollment failed); no enrollment needed (D6). The outcome
// reaches the saga later as OrderLineRefunded / OrderLineRefundFailed.
internal sealed class RefundOrderLineConsumer(ISender mediator, ILogger<RefundOrderLineConsumer> logger) : IConsumer<RefundOrderLine>
{
    public async Task Consume(ConsumeContext<RefundOrderLine> context)
    {
        var msg = context.Message;

        logger.LogInformation(
            "Processing message {messageName}, data: {@message}",
            nameof(RefundOrderLine), msg);

        var result = await mediator.Send(
            new RequestLineRefundCommand(msg.OrderId, msg.OrderLineId, msg.Reason, ByStudent: false), context.CancellationToken);

        if (result.IsSuccess)
        {
            logger.LogInformation(
                "Successfully processed message {messageName}",
                nameof(RefundOrderLine));
        }
        else
        {
            // A business error (e.g. the refund was already requested) is acknowledged, not retried.
            logger.LogWarning(
                "Failed to process message {messageName}. Errors: {@errors}",
                nameof(RefundOrderLine), result.Errors);
        }
    }
}

// Endpoint "payment-refund-order-line": the Ordering saga sends RefundOrderLine there.
internal sealed class RefundOrderLineConsumerDefinition : ConsumerDefinition<RefundOrderLineConsumer>
{
    protected override void ConfigureConsumer(
        IReceiveEndpointConfigurator endpointConfigurator,
        IConsumerConfigurator<RefundOrderLineConsumer> consumerConfigurator,
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
