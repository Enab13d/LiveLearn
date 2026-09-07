using LiveLearn.Assessment.Infrastructure.Contexts;
using LiveLearn.Assessment.Infrastructure.Models;
using LiveLearn.BuildingBlocks;
using LiveLearn.Contracts.Catalog;
using MassTransit;

namespace LiveLearn.Assessment.Infrastructure.Messaging.Consumers;


internal sealed class CoursePublishedConsumer(WriteDbContext dbContext, IUnitOfWork unitOfWork) : IConsumer<CoursePublishedEvent>
{
    public async Task Consume(ConsumeContext<CoursePublishedEvent> context)
    {
        var msg = context.Message;
        var ct = context.CancellationToken;

        var lockedTasks = msg.TaskIds.Select(taskId => new LockedTask()
        {
            TaskId = taskId,
            CourseId = msg.CourseId
        }).ToList();

        await dbContext.LockedTasks.AddRangeAsync(lockedTasks, ct);
        await unitOfWork.CommitAsync(ct);

    }
}

internal sealed class CoursePublishedConsumerDefinition : ConsumerDefinition<CoursePublishedConsumer>
{

    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<CoursePublishedConsumer> consumerConfigurator, IRegistrationContext context)
    {
        endpointConfigurator.UseEntityFrameworkOutbox<WriteDbContext>(context);
    }
}
