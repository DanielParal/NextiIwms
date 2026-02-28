namespace Nexticz.Module.Sign.Settings.Domain.ImportAggregate;

public enum ImportStatus
{
    Success,
    PartialSuccessWithErrors,
    Failure,
    Requested
}