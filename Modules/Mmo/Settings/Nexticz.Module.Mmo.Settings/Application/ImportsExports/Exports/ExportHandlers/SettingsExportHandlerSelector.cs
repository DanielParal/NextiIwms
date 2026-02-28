using Nexticz.Module.Mmo.Settings.Domain.ExportEntity;

namespace Nexticz.Module.Mmo.Settings.Application.ImportsExports.Exports.ExportHandlers;

internal class SettingsExportHandlerSelector(IEnumerable<ISettingsExportHandler<ExportType>> handlers)
    : ISettingsExportHandlerSelector
{
    public ISettingsExportHandler<ExportType>? GetHandler(ExportType exportType)
    {
        return handlers.FirstOrDefault(x => x.CanHandle(exportType));
    }
}