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
            "Processing message {messageName} with correlationId {correlationId}, data: {@message}",
            nameof(RefundCompletedEvent), context.CorrelationId, msg);

        var result = await mediator.Send(new RevokeEnrollmentCommand(msg.StudentId, msg.CourseId, RevocationReason.Refund), context.CancellationToken);

        if (result.IsSuccess)
        {
            logger.LogInformation(
                "Successfully processed message {messageName} with correlation id {correlationId}",
                nameof(RefundCompletedEvent), context.CorrelationId);
        }
        else
        {
            logger.LogWarning(
                "Failed to process message {messageName} with correlation id {correlationId}. Errors: {@errors}",
                nameof(RefundCompletedEvent), context.CorrelationId, result.Errors);
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
