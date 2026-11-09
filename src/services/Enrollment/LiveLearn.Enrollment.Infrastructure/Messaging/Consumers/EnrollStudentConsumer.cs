using LiveLearn.Contracts.Enrollment;
using LiveLearn.Enrollment.Application.Commands;
using LiveLearn.Enrollment.Infrastructure.Contexts;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LiveLearn.Enrollment.Infrastructure.Messaging.Consumers;


// One step of the purchase saga: every outcome is replied to the saga, which moves on from there.
internal sealed class EnrollStudentConsumer(ISender mediator, TimeProvider timeProvider, ILogger<EnrollStudentConsumer> logger)
    : IConsumer<EnrollStudent>
{
    public async Task Consume(ConsumeContext<EnrollStudent> context)
    {
        var msg = context.Message;

        logger.LogInformation(
            "Processing message {messageName}, data: {@message}",
            nameof(EnrollStudent), msg);

        var result = await mediator.Send(
            new EnrollStudentCommand(msg.OrderId, msg.OrderLineId, msg.StudentId, msg.CourseId), context.CancellationToken);

        if (result.IsSuccess)
        {
            await context.Publish(new StudentEnrolled(
                msg.OrderId, msg.OrderLineId, msg.StudentId, msg.CourseId, timeProvider.GetUtcNow()), context.CancellationToken);

            logger.LogInformation(
                "Successfully processed message {messageName}",
                nameof(EnrollStudent));
            return;
        }

        // A business outcome (e.g. AlreadyEnrolled), not a fault: acknowledged; the saga refunds the line.
        await context.Publish(new EnrollmentFailedEvent(
            msg.OrderId, msg.OrderLineId, msg.StudentId, msg.CourseId, result.FirstError.Code, timeProvider.GetUtcNow()),
            context.CancellationToken);

        logger.LogWarning(
            "Failed to process message {messageName}. Errors: {@errors}",
            nameof(EnrollStudent), result.Errors);
    }
}

// Endpoint "enrollment-enroll-student": the Ordering saga sends EnrollStudent there. No retries for now.
internal sealed class EnrollStudentConsumerDefinition : ConsumerDefinition<EnrollStudentConsumer>
{
    protected override void ConfigureConsumer(
        IReceiveEndpointConfigurator endpointConfigurator,
        IConsumerConfigurator<EnrollStudentConsumer> consumerConfigurator,
        IRegistrationContext context)
    {
        endpointConfigurator.UseEntityFrameworkOutbox<WriteDbContext>(context);
    }
}
