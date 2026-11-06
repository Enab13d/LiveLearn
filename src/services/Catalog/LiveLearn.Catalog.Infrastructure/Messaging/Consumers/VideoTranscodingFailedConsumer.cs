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
            "Processing message {messageName}, data: {@message}",
            nameof(VideoTranscodingFailedEvent), msg);

        var result = await mediator.Send(
            new MarkLectureVideoFailedCommand(msg.CourseId, msg.LectureId, msg.VideoId), context.CancellationToken);

        if (result.IsSuccess)
        {
            logger.LogInformation(
                "Successfully processed message {messageName}",
                nameof(VideoTranscodingFailedEvent));
        }
        else
        {
            logger.LogWarning(
                "Failed to process message {messageName}. Errors: {@errors}",
                nameof(VideoTranscodingFailedEvent), result.Errors);
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
