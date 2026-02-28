namespace Nexticz.Module.Sign.DocumentManager.Application.MasstransitPublishers.SignalRNotifications;

internal interface IDocumentPrintingNotifier
{
    Task NotifyDocumentsPrintingStartedAsync(string signingDeviceCode, CancellationToken cancellationToken);
    Task NotifyDocumentsPrintingSucceededAsync(string signingDeviceCode, CancellationToken cancellationToken);
    Task NotifyDocumentsPrintingFailedButWithRetryAsync(string signingDeviceCode, CancellationToken cancellationToken);
    Task NotifyDocumentsPrintingFailedAsync(string signingDeviceCode, CancellationToken cancellationToken);
}