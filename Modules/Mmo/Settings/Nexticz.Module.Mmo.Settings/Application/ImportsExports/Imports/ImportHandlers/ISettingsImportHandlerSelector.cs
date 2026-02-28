using Nexticz.Module.Mmo.Settings.Domain.ImportEntity;

namespace Nexticz.Module.Mmo.Settings.Application.ImportsExports.Imports.ImportHandlers;

internal interface ISettingsImportHandlerSelector
{
    ISettingsImportHandler<ImportType>? GetHandler(ImportType importType);
}