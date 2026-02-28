using MassTransit;
using Nexticz.Module.Notifications.Contracts.SignalRNotifications;
using Nexticz.Lib.Shared.MessagePublishers;
using Nexticz.Module.Cuzk.Domain.ImportAggregate;

namespace Nexticz.Module.Cuzk.Application.MasstransitPublishers;

internal class MasstransitPublisher(IPublishEndpoint publishEndpoint) 
    : BaseMessagePublisher(publishEndpoint), IMasstransitPublisher
{
    public async Task NotifyImportFinishedAsync(Guid importId, ImportType importType, string requestedByUserName, CancellationToken cancellationToken)
    {
        var signalRPublishedNotification = 
            new SignalRPublishedNotification(
                NotificationTranslations.ImportFinishedTitle.TranslationValue,
                NotificationTranslations.ImportFinishedDescription(importId, importType).TranslationValue,
                null, 
                nameof(ReceivableNotification.ImportFinished), 
                ModuleNameProvider.Name,
                NotificationSeverityContract.Success,
                [requestedByUserName]
            );
        
        await PublishAsync(signalRPublishedNotification, cancellationToken);
    }
    
    public async Task NotifyImportFailedAsync(Guid importId, ImportType importType, string requestedByUserName, string errorDescription, string logId, CancellationToken cancellationToken)
    {
        var signalRPublishedNotification = 
            new SignalRPublishedNotification(
                NotificationTranslations.ImportFailedTitle.TranslationValue,
                NotificationTranslations.ImportFailedDescription(importId, importType, errorDescription, logId).TranslationValue,
                null, 
                nameof(ReceivableNotification.ImportFailed), 
                ModuleNameProvider.Name,
                NotificationSeverityContract.Warning,
                [requestedByUserName]
            );
        
        await PublishAsync(signalRPublishedNotification, cancellationToken);
    }
}