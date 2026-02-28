using Nexticz.Module.Notifications.Contracts.SignalRNotifications;
using Nexticz.Module.Sign.SharedKernel.ModuleConfiguration;

namespace Nexticz.Module.Sign.DocumentManager.Application.MasstransitPublishers.SignalRNotifications;

internal class DocumentPrintingNotifier(
    IDocumentManagerPublisher messagePublisher) : IDocumentPrintingNotifier
{
    public async Task NotifyDocumentsPrintingStartedAsync(string signingDeviceCode, CancellationToken cancellationToken)
    {
        var notification = ComposeNotification(
            signingDeviceCode, 
            notificationType: ReceivableNotification.DocumentsPrintingStarted, 
            NotificationTranslations.DocumentsPrintingStartedTitle.TranslationValue, 
            NotificationTranslations.DocumentsPrintingStartedDescription.TranslationValue);
        
        await messagePublisher.PublishAsync(notification, cancellationToken);
    }

    public async Task NotifyDocumentsPrintingSucceededAsync(string signingDeviceCode, CancellationToken cancellationToken)
    {
        var notification = ComposeNotification(
            signingDeviceCode, 
            notificationType: ReceivableNotification.DocumentsPrintingSucceeded, 
            NotificationTranslations.DocumentsPrintingSucceededTitle.TranslationValue, 
            NotificationTranslations.DocumentsPrintingSucceededDescription.TranslationValue);
        
        await messagePublisher.PublishAsync(notification, cancellationToken);
    }

    public async Task NotifyDocumentsPrintingFailedAsync(string signingDeviceCode, CancellationToken cancellationToken)
    {
        var notification = ComposeNotification(
            signingDeviceCode, 
            notificationType: ReceivableNotification.DocumentsPrintingFailed, 
            NotificationTranslations.DocumentsPrintingFailedTitle.TranslationValue, 
            NotificationTranslations.DocumentsPrintingFailedDescription.TranslationValue);
        
        await messagePublisher.PublishAsync(notification, cancellationToken);
    }
    
    public async Task NotifyDocumentsPrintingFailedButWithRetryAsync(string signingDeviceCode, CancellationToken cancellationToken)
    {
        var notification = ComposeNotification(
            signingDeviceCode, 
            notificationType: ReceivableNotification.DocumentsPrintingFailedButWithRetry, 
            NotificationTranslations.DocumentsPrintingFailedButWithRetryTitle.TranslationValue, 
            NotificationTranslations.DocumentsPrintingFailedButWithRetryDescription.TranslationValue);
        
        await messagePublisher.PublishAsync(notification, cancellationToken);
    }

    private static SignalRPublishedNotification ComposeNotification(string signingDeviceCode, ReceivableNotification notificationType, string title, string description)
    {
        return new SignalRPublishedNotification(
            title,
            description,
            null, 
            notificationType.ToString(), 
            ModuleNameProvider.Name,
            NotificationSeverityContract.Info,
            [$"{ModuleNameProvider.Name}_{signingDeviceCode}"]
        );
    }
}