using LiveLearn.Assessment.Infrastructure.Contexts;
using LiveLearn.Assessment.Infrastructure.Models;
using LiveLearn.BuildingBlocks;
using LiveLearn.Contracts.Catalog;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace LiveLearn.Assessment.Infrastructure.Messaging.Consumers;


// Insert-if-absent: redelivery and a concurrent CourseStructureRevisedConsumer are both no-ops on existing rows.
internal sealed class CoursePublishedConsumer(WriteDbContext dbContext, IUnitOfWork unitOfWork) : IConsumer<CoursePublishedEvent>
{
    public async Task Consume(ConsumeContext<CoursePublishedEvent> context)
    {
        var msg = context.Message;
        var ct = context.CancellationToken;

        if (!await dbContext.PublishedCourses.AnyAsync(e => e.CourseId == msg.CourseId, ct))
            dbContext.PublishedCourses.Add(new PublishedCourse { CourseId = msg.CourseId });

        var alreadyLocked = await dbContext.LockedTasks
            .Where(e => e.CourseId == msg.CourseId && msg.TaskIds.Contains(e.TaskId))
            .Select(e => e.TaskId)
            .ToListAsync(ct);

        var lockedTasks = msg.TaskIds
            .Distinct()
            .Except(alreadyLocked)
            .Select(taskId => new LockedTask { TaskId = taskId, CourseId = msg.CourseId });

        dbContext.LockedTasks.AddRange(lockedTasks);
        await unitOfWork.CommitAsync(ct);
    }
}

internal sealed class CoursePublishedConsumerDefinition : ConsumerDefinition<CoursePublishedConsumer>
{

    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<CoursePublishedConsumer> consumerConfigurator, IRegistrationContext context)
    {
        // a duplicate-key race with CourseStructureRevisedConsumer resolves on retry
        endpointConfigurator.UseMessageRetry(cfg =>
        {
            cfg.Exponential(3, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(2));
        });
        endpointConfigurator.UseEntityFrameworkOutbox<WriteDbContext>(context);
    }
}
