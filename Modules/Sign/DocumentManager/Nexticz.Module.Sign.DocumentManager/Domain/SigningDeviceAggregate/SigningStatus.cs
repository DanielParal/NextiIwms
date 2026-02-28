namespace Nexticz.Module.Sign.DocumentManager.Domain.SigningDeviceAggregate;

public enum SigningStatus
{
    Success,
    SuccessWithPrintingError,
    FailureSigningDocument,
    FailureUnexpected,
    SuccessWithRequestedPrint
}