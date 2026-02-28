namespace Nexticz.Module.Sign.Settings.Contracts.OpenApiContracts;

public enum ReceivableNotificationContract
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