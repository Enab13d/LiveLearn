using LiveLearn.BuildingBlocks;
using LiveLearn.Contracts.Catalog;
using LiveLearn.Enrollment.Domain.Enums;
using LiveLearn.Enrollment.Infrastructure.Contexts;
using LiveLearn.Enrollment.Infrastructure.Models;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace LiveLearn.Enrollment.Infrastructure.Messaging.Consumers;


// Updates the structure projection only; existing enrollments are never touched.
// Draft changes (revision 0) are ignored — CoursePublishedConsumer starts the projection from the published
// snapshot. Revisions are applied strictly in order: an already applied revision is a no-op, a revision
// ahead of the next one (or ahead of CoursePublishedEvent) is retried until the earlier ones arrive.
internal sealed class CourseStructureRevisedConsumer(WriteDbContext dbContext, IUnitOfWork unitOfWork) : IConsumer<CourseStructureRevisedEvent>
{
    public async Task Consume(ConsumeContext<CourseStructureRevisedEvent> context)
    {
        var msg = context.Message;
        var ct = context.CancellationToken;

        if (msg.Revision == 0) return;

        var course = await dbContext.CourseRevisions.FirstOrDefaultAsync(e => e.CourseId == msg.CourseId, ct)
            ?? throw new InvalidOperationException(
                $"Course {msg.CourseId} revision {msg.Revision} arrived before CoursePublishedEvent");

        if (msg.Revision <= course.Revision) return;

        if (msg.Revision != course.Revision + 1)
            throw new InvalidOperationException(
                $"Course {msg.CourseId}: revision {msg.Revision} is not subsequent, expected revision {course.Revision + 1}");

        // Revision is a concurrency token: a concurrent apply of the same revision fails here, is retried,
        // and then sees it as already applied.
        course.Revision = msg.Revision;

        AddItems(msg.CourseId, msg.AddedLectureIds, EnrollmentItemKind.Lecture);
        AddItems(msg.CourseId, msg.AddedTaskIds, EnrollmentItemKind.Task);

        var archivedIds = msg.ArchivedLectureIds.Concat(msg.ArchivedTaskIds).ToList();
        if (archivedIds.Count > 0)
        {
            var archived = await dbContext.CourseStructureItems
                .Where(e => e.CourseId == msg.CourseId && archivedIds.Contains(e.ItemId))
                .ToListAsync(ct);
            dbContext.CourseStructureItems.RemoveRange(archived);
        }

        await unitOfWork.CommitAsync(ct);
    }

    private void AddItems(Guid courseId, IReadOnlyList<Guid> itemIds, EnrollmentItemKind kind)
    {
        dbContext.CourseStructureItems.AddRange(itemIds.Select(itemId => new CourseStructureItem
        {
            CourseId = courseId,
            ItemId = itemId,
            Kind = kind,
        }));
    }
}

internal sealed class CourseStructureRevisedConsumerDefinition : ConsumerDefinition<CourseStructureRevisedConsumer>
{
    protected override void ConfigureConsumer(
        IReceiveEndpointConfigurator endpointConfigurator,
        IConsumerConfigurator<CourseStructureRevisedConsumer> consumerConfigurator,
        IRegistrationContext context)
    {
        // covers a revision waiting for an earlier one or for CoursePublishedEvent, and a concurrent apply
        // of the same revision.
        // In-memory retry holds the message, so the endpoint must keep concurrency > 1 for the earlier
        // revision to be consumed meanwhile.
        endpointConfigurator.UseMessageRetry(cfg =>
        {
            cfg.Exponential(10, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(2));
        });
        endpointConfigurator.UseEntityFrameworkOutbox<WriteDbContext>(context);
    }
}
