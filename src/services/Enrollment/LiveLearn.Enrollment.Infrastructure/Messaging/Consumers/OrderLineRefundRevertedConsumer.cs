using LiveLearn.Contracts.Payment;
using LiveLearn.Enrollment.Application.Commands;
using LiveLearn.Enrollment.Infrastructure.Contexts;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LiveLearn.Enrollment.Infrastructure.Messaging.Consumers;


// The refund failed at Stripe, so the access its OrderLineRefunded revoked comes back.
internal sealed class OrderLineRefundRevertedConsumer(ISender mediator, ILogger<OrderLineRefundRevertedConsumer> logger) : IConsumer<OrderLineRefundReverted>
{
    public async Task Consume(ConsumeContext<OrderLineRefundReverted> context)
    {
        var msg = context.Message;

        logger.LogInformation(
            "Processing message {messageName}, data: {@message}",
            nameof(OrderLineRefundReverted), msg);

        var result = await mediator.Send(new RestoreRefundedEnrollmentCommand(msg.StudentId, msg.CourseId, msg.OrderId, msg.OrderLineId), context.CancellationToken);

        if (result.IsSuccess)
        {
            logger.LogInformation(
                "Successfully processed message {messageName}",
                nameof(OrderLineRefundReverted));
        }
        else
        {
            logger.LogWarning(
                "Failed to process message {messageName}. Errors: {@errors}",
                nameof(OrderLineRefundReverted), result.Errors);
        }
    }
}

internal sealed class OrderLineRefundRevertedConsumerDefinition : ConsumerDefinition<OrderLineRefundRevertedConsumer>
{
    protected override void ConfigureConsumer(
        IReceiveEndpointConfigurator endpointConfigurator,
        IConsumerConfigurator<OrderLineRefundRevertedConsumer> consumerConfigurator,
        IRegistrationContext context)
    {
        endpointConfigurator.UseMessageRetry(cfg =>
        {
            cfg.Exponential(3, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(2));
        });
        endpointConfigurator.UseEntityFrameworkOutbox<WriteDbContext>(context);
    }
}
