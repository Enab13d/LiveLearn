using LiveLearn.BuildingBlocks;
using LiveLearn.Catalog.Infrastructure.Contexts;
using LiveLearn.Contracts.Assessment;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace LiveLearn.Catalog.Infrastructure.Messaging.Consumers;


internal sealed class TaskCreatedConsumer(WriteDbContext dbContext, IUnitOfWork unitOfWork) : IConsumer<TaskCreatedEvent>
{
    public async Task Consume(ConsumeContext<TaskCreatedEvent> context)
    {
        var msg = context.Message;
        var ct = context.CancellationToken;
        if (!await dbContext.TaskReplicas.AnyAsync(e => e.TaskId == msg.TaskId, ct))
        {
            await dbContext.TaskReplicas.AddAsync(new(msg.TaskId, msg.TutorId, msg.TaskType), ct);
            await unitOfWork.CommitAsync(ct);
        }

    }
}
