using System.Collections.Concurrent;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Sign.SharedKernel.ModuleConfiguration;
using Nexticz.Module.EmailSender.Contracts;
using Nexticz.Module.Sign.DocumentManager.Application.LoadingDocuments.Commands.AddSentEmail;
using Nexticz.Module.Sign.DocumentManager.Domain.LoadingDocumentAggregate;

namespace Nexticz.Module.Sign.DocumentManager.Infrastructure.Consumers;

internal class EmailConfirmationConsumer(
    ILogger<EmailConfirmationConsumer> logger,
    ISender sender) : IConsumer<EmailConfirmationMessage>
{
    private static readonly ConcurrentDictionary<string, SemaphoreSlim> LoadingDocumentSemaphores = new();
    
    public async Task Consume(ConsumeContext<EmailConfirmationMessage> context)
    {
        if (!string.Equals(context.Message.ModuleName, ModuleNameProvider.Name, StringComparison.InvariantCultureIgnoreCase))
            return;
        
        var loadingDocumentCode = context.Message.InitiatorProperties.FirstOrDefault(x => x.Key == nameof(EmailInitiatorProperties.LoadingDocumentCode)).Value;
        var deliveryDocumentCode = context.Message.InitiatorProperties.FirstOrDefault(x => x.Key == nameof(EmailInitiatorProperties.DeliveryDocumentCode)).Value;

        if (string.IsNullOrWhiteSpace(loadingDocumentCode))
        {
            logger.LogWarning("Sign module - {ConsumerName} - message received, but loading document code is not provided. MessageId: {MessageId}, emailId: {EmailId}.", 
                nameof(EmailConfirmationMessage), context.MessageId, context.Message.EmailId);
            return;
        }
        
        if (context.Message.ToRecipients.Length == 0)
        {
            logger.LogWarning("Sign module - {ConsumerName} - message received, but recipient is not provided. MessageId: {MessageId}, emailId: {EmailId}.", 
                nameof(EmailConfirmationMessage), context.MessageId, context.Message.EmailId);
            return;       
        }
        
        var semaphore = LoadingDocumentSemaphores.GetOrAdd(loadingDocumentCode, _ => new SemaphoreSlim(1, 1));

        try
        {
            var acquired = await semaphore.WaitAsync(TimeSpan.FromSeconds(60));
            if (!acquired)
            {
                logger.LogWarning(
                    "SIGN - DocumentManager - we cannot add sent emails to the loading document. LoadingDocumentCode: {LoadingDocumentCode}, EmailId: {EmailId}." +
                    "Timeout waiting to acquire lock for device.",
                    loadingDocumentCode, context.Message.EmailId);
                return;
            }

            await sender.Send(new AddSentEmailCommand(loadingDocumentCode!, deliveryDocumentCode,
                context.Message.EmailId, context.Message.AttachmentsCount, context.Message.ToRecipients,
                context.Message.ProcessedAt, context.Message.FailureReason));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, 
                "SIGN - DocumentManager - error when adding sent emails to loading document. LoadingDocumentCode: {LoadingDocumentCode}, EmailId: {EmailId}. ErrorMessage: {ErrorMessage}.",
                loadingDocumentCode, context.Message.EmailId, ex.Message);
        }
        finally
        {
            semaphore.Release();
        }
        
    }
}