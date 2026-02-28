using MassTransit;
using Microsoft.Extensions.Logging;
using Nexticz.Module.EmailSender.Contracts;
using Nexticz.Module.EmailSender.Application.Orchestrators;

namespace Nexticz.Module.EmailSender.Infrastructure.Consumers;

internal class QueueEmailConsumer(
    ILogger<QueueEmailConsumer> logger,
    IQueueEmailOrchestrator queueEmailOrchestrator) : IConsumer<QueueEmailMessage>
{
    public async Task Consume(ConsumeContext<QueueEmailMessage> context)
    {
        logger.LogInformation("EmailSender - {ConsumerName} - message received: {MessageId}.", 
            nameof(QueueEmailConsumer), context.MessageId);
        await queueEmailOrchestrator.QueueEmailAsync(context.Message);
    }
}