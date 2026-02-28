namespace Nexticz.Module.Sign.DocumentManager.Application.MasstransitPublishers.SignalRNotifications;

internal enum ReceivableNotification
{
    DocumentsSentToSigningDevice,
    DocumentsReturnedFromSigningDevice,
    DocumentsPrintingStarted,
    DocumentsPrintingSucceeded,
    DocumentsPrintingFailed,
    DocumentsPrintingFailedButWithRetry,
    DocumentsSigningSucceededWithRequestedPrinting,
    DocumentsSigningSucceededWithoutPrinting,
    DocumentsSigningFailed,
    DocumentsSigningFailedUnexpectedly
}