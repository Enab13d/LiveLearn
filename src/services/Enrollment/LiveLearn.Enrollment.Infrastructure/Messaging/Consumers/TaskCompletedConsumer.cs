using LiveLearn.Contracts.Assessment;
using LiveLearn.Enrollment.Application.Commands;
using LiveLearn.Enrollment.Infrastructure.Contexts;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LiveLearn.Enrollment.Infrastructure.Messaging.Consumers;


internal sealed class TaskCompletedConsumer(ISender mediator, ILogger<TaskCompletedConsumer> logger) : IConsumer<TaskCompletedEvent>
{
    public async Task Consume(ConsumeContext<TaskCompletedEvent> context)
    {
        var msg = context.Message;

        logger.LogInformation(
            "Processing message {messageName} with correlationId {correlationId}, data: {@message}",
            nameof(TaskCompletedEvent), context.CorrelationId, msg);

        var result = await mediator.Send(new CompleteTaskCommand(msg.StudentId, msg.CourseId, msg.TaskId), context.CancellationToken);

        if (result.IsSuccess)
        {
            logger.LogInformation(
                "Successfully processed message {messageName} with correlation id {correlationId}",
                nameof(TaskCompletedEvent), context.CorrelationId);
        }
        else
        {
            logger.LogWarning(
                "Failed to process message {messageName} with correlation id {correlationId}. Errors: {@errors}",
                nameof(TaskCompletedEvent), context.CorrelationId, result.Errors);
        }
    }
}

internal sealed class TaskCompletedConsumerDefinition : ConsumerDefinition<TaskCompletedConsumer>
{
    protected override void ConfigureConsumer(
        IReceiveEndpointConfigurator endpointConfigurator,
        IConsumerConfigurator<TaskCompletedConsumer> consumerConfigurator,
        IRegistrationContext context)
    {
        endpointConfigurator.UseMessageRetry(cfg =>
        {
            cfg.Exponential(3, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(2));
        });
        endpointConfigurator.UseEntityFrameworkOutbox<WriteDbContext>(context);
    }
}
