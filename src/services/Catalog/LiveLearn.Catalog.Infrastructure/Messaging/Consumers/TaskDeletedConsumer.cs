using LiveLearn.BuildingBlocks;
using LiveLearn.Catalog.Application.Commands;
using LiveLearn.Catalog.Application.Repositories;
using LiveLearn.Catalog.Infrastructure.Contexts;
using LiveLearn.Contracts.Assessment;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LiveLearn.Catalog.Infrastructure.Messaging.Consumers;


internal sealed class TaskDeletedConsumer(
    ICourseRepository courseRepository,
    WriteDbContext writeDbContext,
    ISender mediator,
    IUnitOfWork unitOfWork) : IConsumer<TaskDeletedEvent>
{
    public async Task Consume(ConsumeContext<TaskDeletedEvent> context)
    {
        var msg = context.Message;
        var ct = context.CancellationToken;

        var replica = await writeDbContext.TaskReplicas.FirstOrDefaultAsync(e => e.TaskId == msg.TaskId, ct);
        if (replica is null) return;

        writeDbContext.TaskReplicas.Remove(replica);
        var course = await courseRepository.GetByTaskIdAsync(msg.TaskId, ct);
        if (course is null)
        {
            await unitOfWork.CommitAsync(ct);
            return;
        }
        
        var sectionId = course.GetIdOfSectionContainingTask(msg.TaskId);
        if (sectionId is not null)
        {
            var command = new RemoveTaskFromSectionCommand(course.TutorId, course.Id, sectionId.Value, msg.TaskId);
            await mediator.Send(command, ct);

        }

    }
}

internal sealed class TaskDeletedConsumerDefinition : ConsumerDefinition<TaskDeletedConsumer>
{

    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<TaskDeletedConsumer> consumerConfigurator, IRegistrationContext context)
    {
        endpointConfigurator.UseEntityFrameworkOutbox<WriteDbContext>(context);
    }
}
