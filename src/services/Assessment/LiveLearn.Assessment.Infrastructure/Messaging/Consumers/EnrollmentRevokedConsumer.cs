using LiveLearn.Assessment.Infrastructure.Contexts;
using LiveLearn.Assessment.Infrastructure.Models;
using LiveLearn.BuildingBlocks;
using LiveLearn.Contracts.Enrollment;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LiveLearn.Assessment.Infrastructure.Messaging.Consumers;

internal sealed class EnrollmentRevokedConsumer(
    WriteDbContext dbContext,
    IUnitOfWork unitOfWork,
    ILogger<EnrollmentRevokedConsumer> logger
) : IConsumer<EnrollmentRevokedEvent>
{
    public async Task Consume(ConsumeContext<EnrollmentRevokedEvent> context)
    {
        var msg = context.Message;
        var ct = context.CancellationToken;

        logger.LogInformation(
            "Processing message {messageName} with correlationId {correlationId}, data: {@message}",
            nameof(EnrollmentRevokedEvent), context.CorrelationId, msg);

        var existing = await dbContext.Enrollments
            .FirstOrDefaultAsync(e =>
                e.StudentId == msg.StudentId
                && e.CourseId == msg.CourseId, ct);

        if (existing is null)
        {
            Enrollment enrollment = new()
            {
                StudentId = msg.StudentId,
                CourseId = msg.CourseId,
                Version = msg.Version,
                UpdatedAt = msg.OccurredOn,
                Status = EnrollmentStatus.Revoked
            };

            dbContext.Enrollments.Add(enrollment);
        }
        else if (existing.Version < msg.Version)
        {
            existing.Version = msg.Version;
            existing.UpdatedAt = msg.OccurredOn;
            existing.Status = EnrollmentStatus.Revoked;
        }
        else
        {
            return;
        }

        await unitOfWork.CommitAsync(ct);

        logger.LogInformation(
            "Successfully processed message {messageName} with correlation id {correlationId}",
            nameof(EnrollmentRevokedEvent), context.CorrelationId);
    }
}

internal sealed class EnrollmentRevokedConsumerDefinition : ConsumerDefinition<EnrollmentRevokedConsumer>
{
    protected override void ConfigureConsumer(
        IReceiveEndpointConfigurator endpointConfigurator,
        IConsumerConfigurator<EnrollmentRevokedConsumer> consumerConfigurator,
        IRegistrationContext context
    )
    {
        endpointConfigurator.UseMessageRetry(cfg =>
        {
            cfg.Exponential(3, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(2));
        });
    }
}
