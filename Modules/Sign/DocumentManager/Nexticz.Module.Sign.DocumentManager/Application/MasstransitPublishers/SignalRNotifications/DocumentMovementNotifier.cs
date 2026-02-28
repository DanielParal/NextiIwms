using MediatR;
using Nexticz.Module.Sign.Settings.Contracts.Depositors.Queries;
using Nexticz.Module.Sign.Settings.Contracts.Users.Queries;
using Nexticz.Module.Sign.SharedKernel.ModuleConfiguration;
using Nexticz.Module.Notifications.Contracts.SignalRNotifications;

namespace Nexticz.Module.Sign.DocumentManager.Application.MasstransitPublishers.SignalRNotifications;

internal class DocumentMovementNotifier(
    IDocumentManagerPublisher messagePublisher,
    ISender sender) : IDocumentMovementNotifier
{
    public async Task NotifyDocumentsSentAsync(string signingDeviceCode, CancellationToken cancellationToken)
    {
        var signalRPublishedNotification = 
            new SignalRPublishedNotification(
                NotificationTranslations.DocumentSentToSigningDeviceTitle.TranslationValue,
                NotificationTranslations.DocumentSentToSigningDeviceDescription.TranslationValue,
                null, 
                nameof(ReceivableNotification.DocumentsSentToSigningDevice), 
                ModuleNameProvider.Name,
                NotificationSeverityContract.Info,
                [$"{ModuleNameProvider.Name}_{signingDeviceCode}"]
            );
        
        await messagePublisher.PublishAsync(signalRPublishedNotification, cancellationToken);
    }

    public async Task NotifyDocumentsReturnedAsync(string signingDeviceCode, CancellationToken cancellationToken)
    {
        var signalRPublishedNotification = 
            new SignalRPublishedNotification(
                NotificationTranslations.DocumentReturnedFromSigningDeviceTitle.TranslationValue,
                NotificationTranslations.DocumentReturnedFromSigningDeviceDescription.TranslationValue,
                null, 
                nameof(ReceivableNotification.DocumentsReturnedFromSigningDevice), 
                ModuleNameProvider.Name,
                NotificationSeverityContract.Info,
                [$"{ModuleNameProvider.Name}_{signingDeviceCode}"]
            );
        
        await messagePublisher.PublishAsync(signalRPublishedNotification, cancellationToken);
    }
}