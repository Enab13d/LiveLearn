using LiveLearn.Assessment.Infrastructure.Contexts;
using LiveLearn.Assessment.Infrastructure.Models;
using LiveLearn.BuildingBlocks;
using LiveLearn.Contracts.Catalog;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace LiveLearn.Assessment.Infrastructure.Messaging.Consumers;


// Records every task ever assigned to the course and locks it if the course is published. Catalog bumps the
// revision only after publishing, so Revision > 0 means published — no dependency on CoursePublishedEvent order.
// ArchivedTaskIds are ignored: an unassigned task stays submittable for the course and stays locked.
internal sealed class CourseStructureRevisedConsumer(WriteDbContext dbContext, IUnitOfWork unitOfWork) : IConsumer<CourseStructureRevisedEvent>
{
    public async Task Consume(ConsumeContext<CourseStructureRevisedEvent> context)
    {
        var msg = context.Message;
        var ct = context.CancellationToken;

        if (msg.AddedTaskIds.Count == 0) return;

        var taskIds = msg.AddedTaskIds.Distinct().ToList();

        var knownTaskIds = await dbContext.CourseTasks
            .Where(e => e.CourseId == msg.CourseId && taskIds.Contains(e.TaskId))
            .Select(e => e.TaskId)
            .ToListAsync(ct);

        dbContext.CourseTasks.AddRange(taskIds
            .Except(knownTaskIds)
            .Select(taskId => new CourseTask { CourseId = msg.CourseId, TaskId = taskId }));

        if (msg.Revision > 0)
        {
            var alreadyLocked = await dbContext.LockedTasks
                .Where(e => e.CourseId == msg.CourseId && taskIds.Contains(e.TaskId))
                .Select(e => e.TaskId)
                .ToListAsync(ct);

            dbContext.LockedTasks.AddRange(taskIds
                .Except(alreadyLocked)
                .Select(taskId => new LockedTask { TaskId = taskId, CourseId = msg.CourseId }));
        }

        await unitOfWork.CommitAsync(ct);
    }
}

internal sealed class CourseStructureRevisedConsumerDefinition : ConsumerDefinition<CourseStructureRevisedConsumer>
{
    protected override void ConfigureConsumer(
        IReceiveEndpointConfigurator endpointConfigurator,
        IConsumerConfigurator<CourseStructureRevisedConsumer> consumerConfigurator,
        IRegistrationContext context)
    {
        // a duplicate-key race with CoursePublishedConsumer resolves on retry
        endpointConfigurator.UseMessageRetry(cfg =>
        {
            cfg.Exponential(3, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(2));
        });
        endpointConfigurator.UseEntityFrameworkOutbox<WriteDbContext>(context);
    }
}
