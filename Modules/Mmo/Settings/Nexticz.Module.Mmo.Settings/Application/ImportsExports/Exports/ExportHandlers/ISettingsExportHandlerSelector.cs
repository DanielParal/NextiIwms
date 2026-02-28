using Nexticz.Module.Mmo.Settings.Domain.ExportEntity;

namespace Nexticz.Module.Mmo.Settings.Application.ImportsExports.Exports.ExportHandlers;

internal interface ISettingsExportHandlerSelector
{
    ISettingsExportHandler<ExportType>? GetHandler(ExportType exportType);
}