using LiveLearn.BuildingBlocks;
using LiveLearn.Contracts.Catalog;
using LiveLearn.Enrollment.Domain.Enums;
using LiveLearn.Enrollment.Infrastructure.Contexts;
using LiveLearn.Enrollment.Infrastructure.Models;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace LiveLearn.Enrollment.Infrastructure.Messaging.Consumers;


// Starts the structure projection from the published snapshot; later revisions come from CourseStructureRevisedEvent.
internal sealed class CoursePublishedConsumer(WriteDbContext dbContext, IUnitOfWork unitOfWork) : IConsumer<CoursePublishedEvent>
{
    public async Task Consume(ConsumeContext<CoursePublishedEvent> context)
    {
        var msg = context.Message;
        var ct = context.CancellationToken;

        if (await dbContext.CourseRevisions.AnyAsync(e => e.CourseId == msg.CourseId, ct)) return;

        dbContext.CourseRevisions.Add(new CourseRevision { CourseId = msg.CourseId, Revision = msg.Revision });
        dbContext.CourseStructureItems.AddRange(msg.LectureIds
            .Select(id => new CourseStructureItem { CourseId = msg.CourseId, ItemId = id, Kind = EnrollmentItemKind.Lecture }));
        dbContext.CourseStructureItems.AddRange(msg.TaskIds
            .Select(id => new CourseStructureItem { CourseId = msg.CourseId, ItemId = id, Kind = EnrollmentItemKind.Task }));

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
        // a concurrent redelivery fails on the CourseId key and is retried into the no-op
        endpointConfigurator.UseMessageRetry(cfg => cfg.Interval(3, TimeSpan.FromSeconds(1)));
        endpointConfigurator.UseEntityFrameworkOutbox<WriteDbContext>(context);
    }
}
