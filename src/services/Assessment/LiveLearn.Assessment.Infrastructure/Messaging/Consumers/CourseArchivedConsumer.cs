using LiveLearn.Assessment.Infrastructure.Contexts;
using LiveLearn.Contracts.Catalog;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace LiveLearn.Assessment.Infrastructure.Messaging.Consumers;


internal sealed class CourseArchivedConsumer(WriteDbContext dbContext) : IConsumer<CourseArchivedEvent>
{
    public async Task Consume(ConsumeContext<CourseArchivedEvent> context)
    {

        await dbContext.LockedTasks
            .Where(e => e.CourseId == context.Message.CourseId)
            .ExecuteDeleteAsync(context.CancellationToken);    

    }
}

internal sealed class CourseArchivedConsumerDefinition : ConsumerDefinition<CourseArchivedConsumer>
{

    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<CourseArchivedConsumer> consumerConfigurator, IRegistrationContext context)
    {
        endpointConfigurator.UseEntityFrameworkOutbox<WriteDbContext>(context);
    }
}
