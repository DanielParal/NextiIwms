using Microsoft.Extensions.Logging;
using MediatR;
using Nexticz.Module.EmailSender.Contracts;
using Nexticz.Lib.Shared.FileHandling.Models;
using Nexticz.Module.EmailSender.Application.EmailComposers;
using Nexticz.Module.EmailSender.Application.EmailMessages.Commands.CreateEmailMessage;
using Nexticz.Module.EmailSender.Application.EmailMessages.Commands.SaveEmailMessageFailed;
using Nexticz.Module.EmailSender.Application.EmailMessages.Commands.SaveEmailMessageSent;
using Nexticz.Module.EmailSender.Application.FileHandling;
using Nexticz.Module.EmailSender.Application.Interfaces;
using Nexticz.Module.EmailSender.Application.Publishers;
using Nexticz.Module.EmailSender.Application.ScheduledEmailMessages.Queries.GetScheduledEmailMessagesToSend;
using Nexticz.Module.EmailSender.Domain.EmailMessageAggregate;

namespace Nexticz.Module.EmailSender.Application.Orchestrators;

internal class SendEmailOrchestrator(
    ILogger<SendEmailOrchestrator> logger,
    ISender sender,
    IEmailComposer composer,
    IEmailSender emailSender,
    IEmailConfirmationPublisher emailConfirmationPublisher,
    IEmailSenderFileHandler fileHandler) : ISendEmailOrchestrator
{
    public async Task SendEmailAsync(SendEmailMessage sendEmailMessage)
    {
        var emailMessage = await sender.Send(new CreateEmailMessageCommand(sendEmailMessage), CancellationToken.None);

        if (emailMessage.IsError)
            return;
        
        await SendAndStoreEmailAsync(
            emailMessage.Value.Id,
            emailMessage.Value.ToRecipients, 
            emailMessage.Value.CcRecipients, 
            emailMessage.Value.BccRecipients, 
            emailMessage.Value.Subject, 
            emailMessage.Value.TextBody, 
            emailMessage.Value.HtmlBody, 
            emailMessage.Value.AttachmentsFolderGuid, 
            emailMessage.Value.Initiator,
            CancellationToken.None);
    }

    public async Task SendScheduledEmailsAsync(string roundKey, CancellationToken cancellationToken)
    {
        var emailMessagesToSend = await sender.Send(new GetScheduledEmailMessagesToSendQuery(), cancellationToken);
        
        foreach (var emailMessageToSend in emailMessagesToSend)
        {
            await SendAndStoreEmailAsync(
                emailMessageToSend.EmailId, emailMessageToSend.ToRecipients, emailMessageToSend.CcRecipients, 
                emailMessageToSend.BccRecipients, emailMessageToSend.Subject, emailMessageToSend.TextBody, 
                emailMessageToSend.HtmlBody, emailMessageToSend.AttachmentsFolderGuid, emailMessageToSend.Initiator, 
                cancellationToken);
        }

        if (emailMessagesToSend.Length > 0)
        {
            logger.LogInformation("EmailSender - RoundKey: {RoundKey} - {EmailsCount} emails sent.", roundKey, emailMessagesToSend.Length);
        }
    }

    private async Task SendAndStoreEmailAsync(
        Guid emailId, string[] toRecipients, string[] ccRecipients, string[] bccRecipients, string subject, string textBody, string htmlBody, 
        string? attachmentsFolderGuid, EmailInitiator initiator, CancellationToken cancellationToken)
    {
        FileResult[] attachments = [];
        
        try
        {
            if (attachmentsFolderGuid is not null)
                attachments = await fileHandler.GetAttachmentsAsync(attachmentsFolderGuid, cancellationToken);
            
            var mimeMessage = composer.Compose(toRecipients, ccRecipients, bccRecipients, subject, textBody, htmlBody, attachments);
            await emailSender.SendAsync(mimeMessage, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "EmailSender - SendEmailOrchestrator - error while sending email");
            var errorMessage = ex.Message;
            if (initiator.ShouldSendConfirmationMessage)
                await emailConfirmationPublisher.PublishEmailConfirmationAsync(emailId, toRecipients, ccRecipients, bccRecipients, attachments.Length, initiator, errorMessage, cancellationToken);
            await sender.Send(new SaveEmailMessageFailedCommand(emailId, errorMessage), cancellationToken);
            throw;
        }
        
        if (initiator.ShouldSendConfirmationMessage)
            await emailConfirmationPublisher.PublishEmailConfirmationAsync(emailId, toRecipients, ccRecipients, bccRecipients, attachments.Length, initiator, null, cancellationToken);
        
        var emailAttachments = attachments.Select(x => new EmailAttachment(x.FileName, x.ContentType)).ToArray();
        await sender.Send(new SaveEmailMessageSentCommand(emailId, emailAttachments), cancellationToken);
        
        if (attachmentsFolderGuid is not null)
            fileHandler.DeleteAttachments(attachmentsFolderGuid);
    }
}