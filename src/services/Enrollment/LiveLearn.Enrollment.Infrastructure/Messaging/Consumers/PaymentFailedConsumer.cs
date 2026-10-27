using LiveLearn.Contracts.Payment;
using LiveLearn.Enrollment.Infrastructure.Contexts;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace LiveLearn.Enrollment.Infrastructure.Messaging.Consumers;


// No enrollment is created for a failed payment; the event is only logged.
internal sealed class PaymentFailedConsumer(ILogger<PaymentFailedConsumer> logger) : IConsumer<PaymentFailedEvent>
{
    public Task Consume(ConsumeContext<PaymentFailedEvent> context)
    {
        var msg = context.Message;

        logger.LogInformation(
            "Payment {transactionId} failed for student {studentId} and course {courseId}: {reason}",
            msg.TransactionId, msg.StudentId, msg.CourseId, msg.Reason);

        return Task.CompletedTask;
    }
}

internal sealed class PaymentFailedConsumerDefinition : ConsumerDefinition<PaymentFailedConsumer>
{
    protected override void ConfigureConsumer(
        IReceiveEndpointConfigurator endpointConfigurator,
        IConsumerConfigurator<PaymentFailedConsumer> consumerConfigurator,
        IRegistrationContext context)
    {
        endpointConfigurator.UseMessageRetry(cfg =>
        {
            cfg.Exponential(3, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(2));
        });
        endpointConfigurator.UseEntityFrameworkOutbox<WriteDbContext>(context);
    }
}
