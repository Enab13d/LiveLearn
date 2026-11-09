using LiveLearn.Contracts.Payment;
using LiveLearn.Enrollment.Application.Commands;
using LiveLearn.Enrollment.Domain.Enums;
using LiveLearn.Enrollment.Infrastructure.Contexts;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LiveLearn.Enrollment.Infrastructure.Messaging.Consumers;


// Revokes only the access this order line granted; a refund of an older line keeps a repurchase active.
internal sealed class OrderLineRefundedConsumer(ISender mediator, ILogger<OrderLineRefundedConsumer> logger) : IConsumer<OrderLineRefunded>
{
    public async Task Consume(ConsumeContext<OrderLineRefunded> context)
    {
        var msg = context.Message;

        logger.LogInformation(
            "Processing message {messageName}, data: {@message}",
            nameof(OrderLineRefunded), msg);

        var result = await mediator.Send(new RevokeEnrollmentCommand(msg.StudentId, msg.CourseId, RevocationReason.Refund, msg.OrderLineId), context.CancellationToken);

        if (result.IsSuccess)
        {
            logger.LogInformation(
                "Successfully processed message {messageName}",
                nameof(OrderLineRefunded));
        }
        else
        {
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
        endpointConfigurator.UseMessageRetry(cfg =>
        {
            cfg.Exponential(3, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(2));
        });
        endpointConfigurator.UseEntityFrameworkOutbox<WriteDbContext>(context);
    }
}
