using LiveLearn.Contracts.Payment;
using LiveLearn.Enrollment.Application.Commands;
using LiveLearn.Enrollment.Domain.Enums;
using LiveLearn.Enrollment.Infrastructure.Contexts;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LiveLearn.Enrollment.Infrastructure.Messaging.Consumers;


internal sealed class RefundCompletedConsumer(ISender mediator, ILogger<RefundCompletedConsumer> logger) : IConsumer<RefundCompletedEvent>
{
    public async Task Consume(ConsumeContext<RefundCompletedEvent> context)
    {
        var msg = context.Message;

        logger.LogInformation(
            "Processing message {messageName}, data: {@message}",
            nameof(RefundCompletedEvent), msg);

        // Single-course refund has no order line; null matches enrollments created without one. Replaced by OrderLineRefundedConsumer.
        var result = await mediator.Send(new RevokeEnrollmentCommand(msg.StudentId, msg.CourseId, RevocationReason.Refund, OrderLineId: null), context.CancellationToken);

        if (result.IsSuccess)
        {
            logger.LogInformation(
                "Successfully processed message {messageName}",
                nameof(RefundCompletedEvent));
        }
        else
        {
            logger.LogWarning(
                "Failed to process message {messageName}. Errors: {@errors}",
                nameof(RefundCompletedEvent), result.Errors);
        }
    }
}

internal sealed class RefundCompletedConsumerDefinition : ConsumerDefinition<RefundCompletedConsumer>
{
    protected override void ConfigureConsumer(
        IReceiveEndpointConfigurator endpointConfigurator,
        IConsumerConfigurator<RefundCompletedConsumer> consumerConfigurator,
        IRegistrationContext context)
    {
        endpointConfigurator.UseMessageRetry(cfg =>
        {
            cfg.Exponential(3, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(2));
        });
        endpointConfigurator.UseEntityFrameworkOutbox<WriteDbContext>(context);
    }
}
