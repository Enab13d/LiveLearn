using LiveLearn.Assessment.Application.Commands;
using LiveLearn.Contracts.Catalog;
using MassTransit;
using MediatR;

namespace LiveLearn.Assessment.Infrastructure.Messaging.Consumers;


internal sealed class TaskRemovedFromSectionConsumer(
    ISender mediator
) : IConsumer<TaskRemovedFromSectionEvent>
{
    public async Task Consume(ConsumeContext<TaskRemovedFromSectionEvent> context)
    {
        await mediator.Send(new UnassignTaskCommand(context.Message.TaskId), context.CancellationToken);

    }
}
