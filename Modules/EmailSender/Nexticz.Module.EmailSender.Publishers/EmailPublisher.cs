using Microsoft.Extensions.Logging;
using Nexticz.Module.EmailSender.Contracts;
using Nexticz.Lib.Shared.FileHandling;
using Nexticz.Lib.Shared.FileHandling.Assets;
using Nexticz.Lib.Shared.MessagePublishers;

namespace Nexticz.Module.EmailSender.Publishers;

public abstract class EmailPublisher(
    IBaseMessagePublisher messagePublisher, 
    IFileHandler fileHandler, 
    AssetsSettings assetsSettings,
    ILogger<EmailPublisher> logger) : IEmailPublisher
{
    public async Task PublishSendEmailAsync(PublishEmailMessage message, CancellationToken cancellationToken)
    {
        var folderGuid = await CopyAttachmentsToEmailSenderAsync(message.Attachments, cancellationToken);

        if (message.ScheduledToBeSentAt is null)
        {
            await PublishSendEmailImmediatelyAsync(message, folderGuid, cancellationToken);
            return;
        }
        
        await PublishScheduledEmailAsync(message, folderGuid, cancellationToken);
    }

    private async Task PublishSendEmailImmediatelyAsync(PublishEmailMessage message, string? folderGuid,
        CancellationToken cancellationToken)
    {
        var emailNotification = 
            new SendEmailMessage(
                message.ToRecipients,
                message.CcRecipients,
                message.BccRecipients,
                message.Subject,
                message.HtmlBody,
                message.TextBody,
                new EmailInitiatorContract(
                    message.ModuleName,
                    message.EmailType,
                    message.ShouldSendConfirmationMessage,
                    message.InitiatorProperties),
                folderGuid
            );
        
        await messagePublisher.PublishAsync(emailNotification, cancellationToken);
    }

    private async Task PublishScheduledEmailAsync(PublishEmailMessage message, string? folderGuid, CancellationToken cancellationToken)
    {
        var emailNotification = 
            new QueueEmailMessage(
                message.ToRecipients,
                message.CcRecipients,
                message.BccRecipients,
                message.Subject,
                message.HtmlBody,
                message.TextBody,
                message.ScheduledToBeSentAt!.Value,
                new EmailInitiatorContract(
                    message.ModuleName,
                    message.EmailType,
                    message.ShouldSendConfirmationMessage,
                    message.InitiatorProperties),
                folderGuid
            );
        
        await messagePublisher.PublishAsync(emailNotification, cancellationToken);
    }

    private async Task<string> CopyAttachmentsToEmailSenderAsync(PublishEmailAttachment[] attachments, CancellationToken cancellationToken)
    {
        var assetsBaseFolder = assetsSettings.BaseFolder;
        var sourceRootFolder = EmailSenderDirectoryNameProvider.RootAttachmentsFolder(assetsBaseFolder);
        var folderGuid = Guid.NewGuid().ToString();
        
        foreach (var attachment in attachments)
        {
            var sourceFolderPath = Path.GetDirectoryName(attachment.FilePath);

            if (string.IsNullOrEmpty(sourceFolderPath) || !File.Exists(attachment.FilePath))
            {
                logger.LogWarning("EmailPublisher - there is no file: {FilePath}. We cannot attach it to the email. Skipping.", attachment.FilePath);
                continue;
            }
            
            var destinationFolderPath = Path.Combine(sourceRootFolder, folderGuid);
            await fileHandler.CopyFileFromSourceToDestinationAsync(attachment.FileName, attachment.NewFileName, sourceFolderPath, destinationFolderPath, false, false, cancellationToken);
        }
        
        return folderGuid;
    }
}