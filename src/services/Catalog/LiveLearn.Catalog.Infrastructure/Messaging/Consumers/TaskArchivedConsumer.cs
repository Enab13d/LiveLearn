using LiveLearn.BuildingBlocks;
using LiveLearn.Catalog.Infrastructure.Contexts;
using LiveLearn.Contracts.Assessment;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace LiveLearn.Catalog.Infrastructure.Messaging.Consumers;


// Only blocks new assignments of the task; existing course links stay so enrolled students keep it.
internal sealed class TaskArchivedConsumer(WriteDbContext dbContext, IUnitOfWork unitOfWork) : IConsumer<TaskArchivedEvent>
{
    public async Task Consume(ConsumeContext<TaskArchivedEvent> context)
    {
        var msg = context.Message;
        var ct = context.CancellationToken;

        var replica = await dbContext.TaskReplicas.FirstOrDefaultAsync(e => e.TaskId == msg.TaskId, ct);
        if (replica is null || replica.IsArchived) return;

        replica.IsArchived = true;
        await unitOfWork.CommitAsync(ct);
    }
}
