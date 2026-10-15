using LiveLearn.Catalog.Infrastructure.Contexts;
using LiveLearn.Catalog.Infrastructure.Models;
using LiveLearn.BuildingBlocks;
using LiveLearn.Contracts.Enrollment;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LiveLearn.Catalog.Infrastructure.Messaging.Consumers;


internal sealed class EnrollmentActivatedConsumer(
    WriteDbContext writeDbContext,
    IUnitOfWork unitOfWork,
    ILogger<EnrollmentActivatedConsumer> logger
) : IConsumer<EnrollmentActivatedEvent>
{
    public async Task Consume(ConsumeContext<EnrollmentActivatedEvent> context)
    {
        var msg = context.Message;
        var ct = context.CancellationToken;

        logger.LogInformation(
            "Processing message {messageName} with correlationId {correlationId}, data: {@message}",
            nameof(EnrollmentActivatedEvent), context.CorrelationId, msg);

        var existing = await writeDbContext.CourseEnrollments
            .FirstOrDefaultAsync(e =>
                e.CourseId == msg.CourseId
                && e.StudentId == msg.StudentId, ct);

        if (existing is null)
        {
            CourseEnrollment enrollment = new()
            {
                StudentId = msg.StudentId,
                CourseId = msg.CourseId,
                Version = msg.Version,
                CourseRevision = msg.CourseRevision,
                UpdatedAt = msg.OccurredOn,
                Status = CourseEnrollmentStatus.Active

            };

            writeDbContext.CourseEnrollments.Add(enrollment);
        }

        else if (existing.Version < msg.Version)
        {
            existing.UpdatedAt = msg.OccurredOn;
            existing.Version = msg.Version;
            existing.Status = CourseEnrollmentStatus.Active;
            existing.CourseRevision = msg.CourseRevision;

        }
        else
        {
            return;
        }

        await unitOfWork.CommitAsync(ct);

        logger.LogInformation(
            "Successfully processed message {messageName} with correlation id {correlationId}",
            nameof(EnrollmentActivatedEvent), context.CorrelationId);
    }
}

internal sealed class EnrollmentActivatedConsumerDefinition : ConsumerDefinition<EnrollmentActivatedConsumer>
{
    protected override void ConfigureConsumer(
        IReceiveEndpointConfigurator endpointConfigurator,
        IConsumerConfigurator<EnrollmentActivatedConsumer> consumerConfigurator,
        IRegistrationContext context)
    {

        endpointConfigurator.UseMessageRetry(cfg =>
        {
            cfg.Exponential(3, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(2));
        });
    }
}
