using Nexticz.Module.Cuzk.Contracts.Imports;
using Nexticz.Module.Cuzk.Domain.ImportAggregate;

namespace Nexticz.Module.Cuzk.Presentation.Imports;

internal static class ImportResponseFactory
{
    public static ImportResponse Create(Import import)
    {
        return new ImportResponse(
            import.Id, 
            import.UserName, 
            import.FileName, 
            (ImportTypeContract)import.Type,
            (ImportStatusContract)import.Status, 
            import.ImportedCodes, 
            import.Errors.Select(x => x.Message).ToArray(),
            import.DateRequested, 
            import.DateImported);
    }
}