using LiveLearn.Contracts.Payment;
using LiveLearn.Enrollment.Application.Commands;
using LiveLearn.Enrollment.Domain.Errors;
using LiveLearn.Enrollment.Infrastructure.Contexts;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LiveLearn.Enrollment.Infrastructure.Messaging.Consumers;


internal sealed class PaymentConfirmedConsumer(ISender mediator, ILogger<PaymentConfirmedConsumer> logger) : IConsumer<PaymentConfirmedEvent>
{
    public async Task Consume(ConsumeContext<PaymentConfirmedEvent> context)
    {
        var msg = context.Message;

        logger.LogInformation(
            "Processing message {messageName} with correlationId {correlationId}, data: {@message}",
            nameof(PaymentConfirmedEvent), context.CorrelationId, msg);

        var result = await mediator.Send(new CreateEnrollmentCommand(msg.StudentId, msg.CourseId), context.CancellationToken);

        if (result.IsSuccess)
        {
            logger.LogInformation(
                "Successfully processed message {messageName} with correlation id {correlationId}",
                nameof(PaymentConfirmedEvent), context.CorrelationId);
            return;
        }

        // Transient: CoursePublishedEvent hasn't reached the structure projection yet, so let the retry kick in.
        if (result.Errors.Any(e => e.Code == EnrollmentErrors.CourseStructureUnavailable.Code))
            throw new InvalidOperationException(
                $"Course {msg.CourseId} structure is not available yet for student {msg.StudentId}");

        logger.LogWarning(
            "Failed to process message {messageName} with correlation id {correlationId}. Errors: {@errors}",
            nameof(PaymentConfirmedEvent), context.CorrelationId, result.Errors);
    }
}

internal sealed class PaymentConfirmedConsumerDefinition : ConsumerDefinition<PaymentConfirmedConsumer>
{
    protected override void ConfigureConsumer(
        IReceiveEndpointConfigurator endpointConfigurator,
        IConsumerConfigurator<PaymentConfirmedConsumer> consumerConfigurator,
        IRegistrationContext context)
    {
        // Covers a payment confirmed before CoursePublishedEvent reached the structure projection.
        endpointConfigurator.UseMessageRetry(cfg =>
        {
            cfg.Exponential(10, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(2));
        });
        endpointConfigurator.UseEntityFrameworkOutbox<WriteDbContext>(context);
    }
}
