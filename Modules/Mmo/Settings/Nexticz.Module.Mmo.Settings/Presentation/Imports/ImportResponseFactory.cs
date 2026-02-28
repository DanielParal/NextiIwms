using Nexticz.Module.Mmo.Settings.Contracts.Imports;
using Nexticz.Module.Mmo.Settings.Domain.ImportEntity;

namespace Nexticz.Module.Mmo.Settings.Presentation.Imports;

internal class ImportResponseFactory
{
    public static ImportResponse Create(Import import)
    {
        var status = (ImportStatusContract)import.Status;
        var type = (ImportTypeContract)import.Type;
        var contractErrors = import.Errors.Select(e => new Contracts.Imports.ImportError(e.Code, e.Message)).ToArray();
        return new ImportResponse(import.Id, import.UserName, status, type, import.ImportedCodes.Length, contractErrors, import.DateCreated);
    }
}