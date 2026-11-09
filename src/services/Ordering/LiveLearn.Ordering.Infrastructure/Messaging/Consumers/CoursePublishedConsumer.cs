using LiveLearn.BuildingBlocks;
using LiveLearn.Contracts.Catalog;
using LiveLearn.Ordering.Infrastructure.Contexts;
using LiveLearn.Ordering.Infrastructure.Models;
using MassTransit;

namespace LiveLearn.Ordering.Infrastructure.Messaging.Consumers;


// Upserts the course: owner, status Published unless already Archived, price only if newer than PriceUpdatedAt.
internal sealed class CoursePublishedConsumer(WriteDbContext dbContext, IUnitOfWork unitOfWork) : IConsumer<CoursePublishedEvent>
{
    public async Task Consume(ConsumeContext<CoursePublishedEvent> context)
    {
        var msg = context.Message;
        var ct = context.CancellationToken;

        var course = await dbContext.CourseProjections.FindAsync([msg.CourseId], ct);
        if (course is null)
        {
            course = CourseProjection.Create(msg.CourseId);
            dbContext.CourseProjections.Add(course);
        }

        course.ApplyPublished(msg.TutorId, msg.Price, msg.PublishedAt);

        await unitOfWork.CommitAsync(ct);
    }
}

internal sealed class CoursePublishedConsumerDefinition : ConsumerDefinition<CoursePublishedConsumer>
{
    protected override void ConfigureConsumer(
        IReceiveEndpointConfigurator endpointConfigurator,
        IConsumerConfigurator<CoursePublishedConsumer> consumerConfigurator,
        IRegistrationContext context)
    {
        // a concurrent insert of the same CourseId or an xmin conflict is retried against fresh state
        endpointConfigurator.UseMessageRetry(cfg => cfg.Interval(3, TimeSpan.FromSeconds(1)));
        endpointConfigurator.UseEntityFrameworkOutbox<WriteDbContext>(context);
    }
}
