using LiveLearn.BuildingBlocks;
using LiveLearn.Contracts.Catalog;
using LiveLearn.Ordering.Infrastructure.Contexts;
using LiveLearn.Ordering.Infrastructure.Models;
using MassTransit;

namespace LiveLearn.Ordering.Infrastructure.Messaging.Consumers;


// Upserts the price only if newer than PriceUpdatedAt. May arrive before CoursePublishedEvent — the row then
// stays Unknown (not purchasable) until the publish arrives.
internal sealed class CoursePriceChangedConsumer(WriteDbContext dbContext, IUnitOfWork unitOfWork) : IConsumer<CoursePriceChangedEvent>
{
    public async Task Consume(ConsumeContext<CoursePriceChangedEvent> context)
    {
        var msg = context.Message;
        var ct = context.CancellationToken;

        var course = await dbContext.CourseProjections.FindAsync([msg.CourseId], ct);
        if (course is null)
        {
            course = CourseProjection.Create(msg.CourseId);
            dbContext.CourseProjections.Add(course);
        }

        course.ApplyPrice(msg.NewPrice, msg.OccurredOn);

        await unitOfWork.CommitAsync(ct);
    }
}

internal sealed class CoursePriceChangedConsumerDefinition : ConsumerDefinition<CoursePriceChangedConsumer>
{
    protected override void ConfigureConsumer(
        IReceiveEndpointConfigurator endpointConfigurator,
        IConsumerConfigurator<CoursePriceChangedConsumer> consumerConfigurator,
        IRegistrationContext context)
    {
        // a concurrent insert of the same CourseId or an xmin conflict is retried against fresh state
        endpointConfigurator.UseMessageRetry(cfg => cfg.Interval(3, TimeSpan.FromSeconds(1)));
        endpointConfigurator.UseEntityFrameworkOutbox<WriteDbContext>(context);
    }
}
