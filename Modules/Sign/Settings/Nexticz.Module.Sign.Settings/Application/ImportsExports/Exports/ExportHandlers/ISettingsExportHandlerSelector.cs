using Nexticz.Module.Sign.Settings.Domain.ExportAggregate;

namespace Nexticz.Module.Sign.Settings.Application.ImportsExports.Exports.ExportHandlers;

internal interface ISettingsExportHandlerSelector
{
    ISettingsExportHandler<ExportType>? GetHandler(ExportType exportType);
}