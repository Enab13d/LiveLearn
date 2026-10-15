using LiveLearn.Catalog.Application.Commands;
using LiveLearn.Contracts.Streaming;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LiveLearn.Catalog.Infrastructure.Messaging.Consumers;


internal sealed class VideoTranscodingCompletedConsumer(ISender mediator, ILogger<VideoTranscodingCompletedConsumer> logger) : IConsumer<VideoTranscodingCompletedEvent>
{
    public async Task Consume(ConsumeContext<VideoTranscodingCompletedEvent> context)
    {
        var msg = context.Message;

        logger.LogInformation(
            "Processing message {messageName} with correlationId {correlationId}, data: {@message}",
            nameof(VideoTranscodingCompletedEvent), context.CorrelationId, msg);

        var result = await mediator.Send(
            new MarkLectureVideoReadyCommand(
                msg.CourseId, msg.LectureId, msg.VideoId, msg.Duration),
                context.CancellationToken);

        if (result.IsSuccess)
        {
            logger.LogInformation(
                "Successfully processed message {messageName} with correlation id {correlationId}",
                nameof(VideoTranscodingCompletedEvent), context.CorrelationId);
        }
        else
        {
            logger.LogWarning(
                "Failed to process message {messageName} with correlation id {correlationId}. Errors: {@errors}",
                nameof(VideoTranscodingCompletedEvent), context.CorrelationId, result.Errors);
        }
    }
}

internal sealed class VideoTranscodingCompletedConsumerDefinition : ConsumerDefinition<VideoTranscodingCompletedConsumer>
{
    protected override void ConfigureConsumer(
        IReceiveEndpointConfigurator endpointConfigurator, 
        IConsumerConfigurator<VideoTranscodingCompletedConsumer> consumerConfigurator, 
        IRegistrationContext context)
    {
        endpointConfigurator.UseMessageRetry(cfg =>
        {
            cfg.Exponential(3, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(2));
        });
    }
}
