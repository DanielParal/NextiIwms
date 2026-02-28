using MassTransit;
using Microsoft.Extensions.Logging;
using Nexticz.Module.EmailSender.Contracts;
using Nexticz.Module.EmailSender.Application.Orchestrators;

namespace Nexticz.Module.EmailSender.Infrastructure.Consumers;

internal class SendEmailConsumer(
    ILogger<SendEmailConsumer> logger,
    ISendEmailOrchestrator sendEmailOrchestrator) : IConsumer<SendEmailMessage>
{
    public async Task Consume(ConsumeContext<SendEmailMessage> context)
    {
        logger.LogInformation("EmailSender - {ConsumerName} - message received: {MessageId}.", 
            nameof(SendEmailConsumer), context.MessageId);
        await sendEmailOrchestrator.SendEmailAsync(context.Message);
    }
}