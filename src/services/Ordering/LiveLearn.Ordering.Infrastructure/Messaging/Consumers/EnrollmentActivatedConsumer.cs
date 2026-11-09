using LiveLearn.BuildingBlocks;
using LiveLearn.Contracts.Enrollment;
using LiveLearn.Ordering.Infrastructure.Contexts;
using LiveLearn.Ordering.Infrastructure.Models;
using MassTransit;

namespace LiveLearn.Ordering.Infrastructure.Messaging.Consumers;


// Upserts IsActive = true only if msg.Version is newer than the stored Version; duplicates and late events are skipped.
internal sealed class EnrollmentActivatedConsumer(WriteDbContext dbContext, IUnitOfWork unitOfWork) : IConsumer<EnrollmentActivatedEvent>
{
    public async Task Consume(ConsumeContext<EnrollmentActivatedEvent> context)
    {
        var msg = context.Message;
        var ct = context.CancellationToken;

        var enrollment = await dbContext.EnrollmentProjections.FindAsync([msg.StudentId, msg.CourseId], ct);
        if (enrollment is null)
            dbContext.EnrollmentProjections.Add(
                EnrollmentProjection.Create(msg.StudentId, msg.CourseId, isActive: true, msg.Version));
        else if (!enrollment.Apply(isActive: true, msg.Version))
            return;

        await unitOfWork.CommitAsync(ct);
    }
}

internal sealed class EnrollmentActivatedConsumerDefinition : ConsumerDefinition<EnrollmentActivatedConsumer>
{
    protected override void ConfigureConsumer(
        IReceiveEndpointConfigurator endpointConfigurator,
        IConsumerConfigurator<EnrollmentActivatedConsumer> consumerConfigurator,
        IRegistrationContext context)
    {
        // a concurrent insert of the same (StudentId, CourseId) or a Version conflict is retried against fresh state
        endpointConfigurator.UseMessageRetry(cfg => cfg.Interval(3, TimeSpan.FromSeconds(1)));
        endpointConfigurator.UseEntityFrameworkOutbox<WriteDbContext>(context);
    }
}
