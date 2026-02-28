namespace Nexticz.Module.Cuzk.Domain.ImportAggregate;

public enum ImportStatus
{
    ImportSucceeded,
    ImportedSucceededWithErrors,
    ImportFailed,
    Requested
}