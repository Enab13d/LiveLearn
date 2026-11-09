using LiveLearn.BuildingBlocks;
using LiveLearn.Contracts.Catalog;
using LiveLearn.Ordering.Infrastructure.Contexts;
using LiveLearn.Ordering.Infrastructure.Models;
using MassTransit;

namespace LiveLearn.Ordering.Infrastructure.Messaging.Consumers;


// Upserts status Archived — terminal, a later CoursePublishedEvent never downgrades it.
internal sealed class CourseArchivedConsumer(WriteDbContext dbContext, IUnitOfWork unitOfWork) : IConsumer<CourseArchivedEvent>
{
    public async Task Consume(ConsumeContext<CourseArchivedEvent> context)
    {
        var msg = context.Message;
        var ct = context.CancellationToken;

        var course = await dbContext.CourseProjections.FindAsync([msg.CourseId], ct);
        if (course is null)
        {
            course = CourseProjection.Create(msg.CourseId);
            dbContext.CourseProjections.Add(course);
        }

        course.Archive();

        await unitOfWork.CommitAsync(ct);
    }
}

internal sealed class CourseArchivedConsumerDefinition : ConsumerDefinition<CourseArchivedConsumer>
{
    protected override void ConfigureConsumer(
        IReceiveEndpointConfigurator endpointConfigurator,
        IConsumerConfigurator<CourseArchivedConsumer> consumerConfigurator,
        IRegistrationContext context)
    {
        // a concurrent insert of the same CourseId or an xmin conflict is retried against fresh state
        endpointConfigurator.UseMessageRetry(cfg => cfg.Interval(3, TimeSpan.FromSeconds(1)));
        endpointConfigurator.UseEntityFrameworkOutbox<WriteDbContext>(context);
    }
}
