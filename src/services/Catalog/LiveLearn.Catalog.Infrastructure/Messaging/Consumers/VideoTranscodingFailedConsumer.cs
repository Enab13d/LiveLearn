using LiveLearn.Catalog.Application.Commands;
using LiveLearn.Contracts.Streaming;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LiveLearn.Catalog.Infrastructure.Messaging.Consumers;


internal sealed class VideoTranscodingFailedConsumer(ISender mediator, ILogger<VideoTranscodingFailedConsumer> logger) : IConsumer<VideoTranscodingFailedEvent>
{
    public async Task Consume(ConsumeContext<VideoTranscodingFailedEvent> context)
    {
        var msg = context.Message;

        logger.LogInformation(
            "Processing message {messageName} with correlationId {correlationId}, data: {@message}",
            nameof(VideoTranscodingFailedEvent), context.CorrelationId, msg);

        var result = await mediator.Send(
            new MarkLectureVideoFailedCommand(msg.CourseId, msg.LectureId, msg.VideoId), context.CancellationToken);

        if (result.IsSuccess)
        {
            logger.LogInformation(
                "Successfully processed message {messageName} with correlation id {correlationId}",
                nameof(VideoTranscodingFailedEvent), context.CorrelationId);
        }
        else
        {
            logger.LogWarning(
                "Failed to process message {messageName} with correlation id {correlationId}. Errors: {@errors}",
                nameof(VideoTranscodingFailedEvent), context.CorrelationId, result.Errors);
        }
    }
}

internal sealed class VideoTranscodingFailedConsumerDefinition : ConsumerDefinition<VideoTranscodingFailedConsumer>
{
    protected override void ConfigureConsumer(
        IReceiveEndpointConfigurator endpointConfigurator,
        IConsumerConfigurator<VideoTranscodingFailedConsumer> consumerConfigurator,
        IRegistrationContext context)
    {
        endpointConfigurator.UseMessageRetry(cfg =>
        {
            cfg.Exponential(3, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(2));
        });
    }
}
