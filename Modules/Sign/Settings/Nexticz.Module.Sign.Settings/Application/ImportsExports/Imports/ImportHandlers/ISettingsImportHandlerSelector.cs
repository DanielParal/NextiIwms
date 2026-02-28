using Nexticz.Module.Sign.Settings.Domain.ImportAggregate;

namespace Nexticz.Module.Sign.Settings.Application.ImportsExports.Imports.ImportHandlers;

internal interface ISettingsImportHandlerSelector
{
    ISettingsImportHandler<ImportType>? GetHandler(ImportType importType);
}