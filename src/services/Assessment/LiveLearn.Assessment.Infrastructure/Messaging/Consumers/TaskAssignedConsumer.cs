using LiveLearn.Assessment.Application.Commands;
using LiveLearn.Contracts.Catalog;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LiveLearn.Assessment.Infrastructure.Messaging.Consumers;


internal sealed class TaskAssignedConsumer(ISender mediator, ILogger<TaskAssignedConsumer> logger) : IConsumer<TaskAssignedToSectionEvent>
{
    public async Task Consume(ConsumeContext<TaskAssignedToSectionEvent> context)
    {
        var msg = context.Message;
        var ct = context.CancellationToken;

        logger.LogInformation(
            "Processing integration event {evtName} with correllationId: {correlationId}", 
            nameof(TaskAssignedToSectionEvent),
            context.CorrelationId
        );

        await mediator.Send(
            new AssignTaskToCourseAndSectionCommand(msg.TaskId, msg.CourseId, msg.SectionId), ct);

    }
}
