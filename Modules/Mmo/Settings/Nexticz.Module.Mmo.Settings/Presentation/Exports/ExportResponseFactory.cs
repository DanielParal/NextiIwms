using Nexticz.Module.Mmo.Settings.Contracts.Exports;
using Nexticz.Module.Mmo.Settings.Domain.ExportEntity;

namespace Nexticz.Module.Mmo.Settings.Presentation.Exports;

internal class ExportResponseFactory
{
    public static ExportResponse Create(Export export)
    {
        var type = (ExportTypeContract)export.Type;
        return new ExportResponse(export.Id, export.UserName, type, export.DateCreated);
    }
}