namespace Nexticz.Module.Sign.DocumentManager.Application.MasstransitPublishers.SignalRNotifications;

internal interface IDocumentMovementNotifier
{
    Task NotifyDocumentsSentAsync(string signingDeviceCode, CancellationToken cancellationToken);
    Task NotifyDocumentsReturnedAsync(string signingDeviceCode, CancellationToken cancellationToken);
}