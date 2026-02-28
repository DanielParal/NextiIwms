using Nexticz.Module.Sign.Settings.Domain.ExportAggregate;

namespace Nexticz.Module.Sign.Settings.Application.ImportsExports.Exports.ExportHandlers;

internal class SettingsExportHandlerSelector(IEnumerable<ISettingsExportHandler<ExportType>> handlers)
    : ISettingsExportHandlerSelector
{
    public ISettingsExportHandler<ExportType>? GetHandler(ExportType exportType)
    {
        return handlers.FirstOrDefault(x => x.CanHandle(exportType));
    }
}