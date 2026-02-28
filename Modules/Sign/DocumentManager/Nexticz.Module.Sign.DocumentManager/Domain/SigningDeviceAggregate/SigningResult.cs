namespace Nexticz.Module.Sign.DocumentManager.Domain.SigningDeviceAggregate;

public record SigningResult(SigningStatus Status, string? ErrorMessage)
{
    public bool IsFailure => Status is SigningStatus.FailureSigningDocument or SigningStatus.FailureUnexpected;
}