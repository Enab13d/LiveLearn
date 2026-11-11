using LiveLearn.Payment.Application.Commands;
using LiveLearn.Payment.Application.Messages;
using LiveLearn.Payment.Infrastructure.Contexts;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LiveLearn.Payment.Infrastructure.Messaging.Consumers;


internal sealed class CreateCheckoutSessionConsumer(ISender mediator, ILogger<CreateCheckoutSessionConsumer> logger)
    : IConsumer<CreateCheckoutSession>
{
    public async Task Consume(ConsumeContext<CreateCheckoutSession> context)
    {
        var msg = context.Message;

        logger.LogInformation(
            "Processing message {messageName}, data: {@message}",
            nameof(CreateCheckoutSession), msg);

        var result = await mediator.Send(new CreateCheckoutSessionCommand(msg.OrderId), context.CancellationToken);

        if (result.IsSuccess)
        {
            logger.LogInformation(
                "Successfully processed message {messageName}",
                nameof(CreateCheckoutSession));
            return;
        }

        // Acknowledged, not retried: a business failure won't change on redelivery.
        logger.LogWarning(
            "Failed to process message {messageName}. Errors: {@errors}",
            nameof(CreateCheckoutSession), result.Errors);
    }
}

internal sealed class CreateCheckoutSessionConsumerDefinition : ConsumerDefinition<CreateCheckoutSessionConsumer>
{
    protected override void ConfigureConsumer(
        IReceiveEndpointConfigurator endpointConfigurator,
        IConsumerConfigurator<CreateCheckoutSessionConsumer> consumerConfigurator,
        IRegistrationContext context)
    {
        // transient Stripe errors (network, 429, 5xx) and xmin conflicts are retried; the idempotency key
        // keeps a retried create from opening a second session (D14)
        endpointConfigurator.UseMessageRetry(cfg => cfg.Interval(3, TimeSpan.FromSeconds(1)));
        endpointConfigurator.UseEntityFrameworkOutbox<WriteDbContext>(context);
    }
}
