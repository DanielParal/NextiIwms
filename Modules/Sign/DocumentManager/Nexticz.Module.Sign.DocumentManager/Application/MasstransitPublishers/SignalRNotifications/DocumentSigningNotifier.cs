using MediatR;
using Nexticz.Lib.Shared.Logging;
using Nexticz.Module.Notifications.Contracts.SignalRNotifications;
using Nexticz.Module.Sign.Settings.Contracts.Depositors.Queries;
using Nexticz.Module.Sign.Settings.Contracts.Users;
using Nexticz.Module.Sign.Settings.Contracts.Users.Queries;
using Nexticz.Module.Sign.SharedKernel.ModuleConfiguration;

namespace Nexticz.Module.Sign.DocumentManager.Application.MasstransitPublishers.SignalRNotifications;

internal class DocumentSigningNotifier(
    IDocumentManagerPublisher messagePublisher,
    ISender sender) : IDocumentSigningNotifier
{
    public async Task NotifyDocumentsSigningSucceededAsync(string signingDeviceCode, string[] depositorCodes, ReceivableNotification notificationType, CancellationToken cancellationToken)
    {
        var receivers = await GetReceivers(signingDeviceCode, depositorCodes, cancellationToken);
        
        var notification = new SignalRPublishedNotification(
            NotificationTranslations.DocumentsSigningSucceededTitle.TranslationValue, 
            NotificationTranslations.DocumentsSigningSucceededDescription.TranslationValue,
            null, 
            notificationType.ToString(), 
            ModuleNameProvider.Name,
            NotificationSeverityContract.Info,
            receivers
        );
        
        await messagePublisher.PublishAsync(notification, cancellationToken);
    }

    public async Task NotifyDocumentsSigningFailedUnexpectedlyAsync(string signingDeviceCode, string[] depositorCodes, CancellationToken cancellationToken)
    {
        var receivers = await GetReceivers(signingDeviceCode, depositorCodes, cancellationToken);
        
        var notification = new SignalRPublishedNotification(
            NotificationTranslations.DocumentsSigningFailedUnexpectedlyTitle.TranslationValue, 
            NotificationTranslations.DocumentsSigningFailedUnexpectedlyDescription(CorrelationIdProvider.Instance.GetInternalId()).TranslationValue,
            null, 
            nameof(ReceivableNotification.DocumentsSigningFailedUnexpectedly), 
            ModuleNameProvider.Name,
            NotificationSeverityContract.Error,
            receivers
        );
        
        await messagePublisher.PublishAsync(notification, cancellationToken);
    }

    public async Task NotifyDocumentsSigningFailedAsync(string signingDeviceCode, string[] depositorCodes, string errorMessage, CancellationToken cancellationToken)
    {
        var receivers = await GetReceivers(signingDeviceCode, depositorCodes, cancellationToken);
        
        var notification = new SignalRPublishedNotification(
            NotificationTranslations.DocumentsSigningFailedTitle.TranslationValue, 
            NotificationTranslations.DocumentsSigningFailedDescription(errorMessage).TranslationValue,
            null, 
            nameof(ReceivableNotification.DocumentsSigningFailed), 
            ModuleNameProvider.Name,
            NotificationSeverityContract.Error,
            receivers
        );
        
        await messagePublisher.PublishAsync(notification, cancellationToken);
    }

    private async Task<string[]> GetReceivers(string signingDeviceCode, string[] depositorCodes, CancellationToken cancellationToken)
    {
        var usersToBeNotified = await GetUsersToBeNotifiedAsync(depositorCodes, cancellationToken);
        var receivers = usersToBeNotified.Select(x => x.UserName)
            .Append($"{ModuleNameProvider.Name}_{signingDeviceCode}")
            .ToArray();
        return receivers;
    }
    
    private async Task<UserResponse[]> GetUsersToBeNotifiedAsync(string[] depositorCodes, CancellationToken cancellationToken)
    {
        var upperDistinctCodes = depositorCodes.Select(x => x.ToUpperInvariant()).Distinct().ToArray();
        var depositorGroupCodes = await sender.Send(new GetDepositorGroupCodesByCodesQuery(upperDistinctCodes), cancellationToken);
        var userResponses = await sender.Send(new GetUserResponsesByDepositorAndGroupCodesQuery(upperDistinctCodes, depositorGroupCodes), cancellationToken);
        return userResponses;
    }
}