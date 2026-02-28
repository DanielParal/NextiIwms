using Nexticz.Module.Sign.Settings.Domain.ImportAggregate;

namespace Nexticz.Module.Sign.Settings.Application.ImportsExports.Imports.ImportHandlers;

internal class SettingsImportHandlerSelector(IEnumerable<ISettingsImportHandler<ImportType>> handlers) : ISettingsImportHandlerSelector
{
    public ISettingsImportHandler<ImportType>? GetHandler(ImportType exportType)
    {
        return handlers.FirstOrDefault(x => x.CanHandle(exportType));
    }
}