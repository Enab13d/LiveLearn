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
            "Processing message {messageName}, data: {@message}",
            nameof(TaskCompletedEvent), msg);

        var result = await mediator.Send(new CompleteTaskCommand(msg.StudentId, msg.CourseId, msg.TaskId), context.CancellationToken);

        if (result.IsSuccess)
        {
            logger.LogInformation(
                "Successfully processed message {messageName}",
                nameof(TaskCompletedEvent));
        }
        else
        {
            logger.LogWarning(
                "Failed to process message {messageName}. Errors: {@errors}",
                nameof(TaskCompletedEvent), result.Errors);
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
