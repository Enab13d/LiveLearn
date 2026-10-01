using LiveLearn.Assessment.Infrastructure.Contexts;
using LiveLearn.Assessment.Infrastructure.Models;
using LiveLearn.BuildingBlocks;
using LiveLearn.Contracts.Enrollment;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace LiveLearn.Assessment.Infrastructure.Messaging.Consumers;


internal sealed class EnrollmentActivatedConsumer(
    WriteDbContext writeDbContext,
    IUnitOfWork unitOfWork) : IConsumer<EnrollmentActivatedEvent>
{
    public async Task Consume(ConsumeContext<EnrollmentActivatedEvent> context)
    {
        var msg = context.Message;
        var ct = context.CancellationToken;

        var existing = await writeDbContext.Enrollments
            .FirstOrDefaultAsync(e =>
                e.CourseId == msg.CourseId
                && e.StudentId == msg.StudentId, ct);

        if (existing is null)
        {
            Enrollment enrollment = new()
            {
                StudentId = msg.StudentId,
                CourseId = msg.CourseId,
                Version = msg.Version,
                UpdatedAt = msg.OccurredOn,
                Status = EnrollmentStatus.Active

            };
            
            writeDbContext.Enrollments.Add(enrollment);
        }
        //if exist compare versions and insert fresher
        else if (existing.Version < msg.Version)
        {
            existing.UpdatedAt = msg.OccurredOn;
            existing.Version = msg.Version;
            existing.Status = EnrollmentStatus.Active;

        }
        else
        {
            return;
        }

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

        endpointConfigurator.UseMessageRetry(cfg =>
        {
            cfg.Exponential(3, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(2));
        });
    }
}
