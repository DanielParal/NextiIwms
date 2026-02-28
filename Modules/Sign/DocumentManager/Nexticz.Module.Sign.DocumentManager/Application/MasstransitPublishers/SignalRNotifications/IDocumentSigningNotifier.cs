namespace Nexticz.Module.Sign.DocumentManager.Application.MasstransitPublishers.SignalRNotifications;

internal interface IDocumentSigningNotifier
{
    Task NotifyDocumentsSigningSucceededAsync(string signingDeviceCode, string[] depositorCodes, ReceivableNotification notificationType, CancellationToken cancellationToken);
    Task NotifyDocumentsSigningFailedUnexpectedlyAsync(string signingDeviceCode, string[] depositorCodes, CancellationToken cancellationToken);
    Task NotifyDocumentsSigningFailedAsync(string signingDeviceCode, string[] depositorCodes, string errorMessage, CancellationToken cancellationToken);
}