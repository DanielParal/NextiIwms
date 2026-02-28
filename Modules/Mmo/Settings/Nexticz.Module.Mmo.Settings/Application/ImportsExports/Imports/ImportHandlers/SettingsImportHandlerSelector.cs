using Nexticz.Module.Mmo.Settings.Domain.ImportEntity;

namespace Nexticz.Module.Mmo.Settings.Application.ImportsExports.Imports.ImportHandlers;

internal class SettingsImportHandlerSelector(IEnumerable<ISettingsImportHandler<ImportType>> handlers) : ISettingsImportHandlerSelector
{
    public ISettingsImportHandler<ImportType>? GetHandler(ImportType importType)
    {
        return handlers.FirstOrDefault(x => x.CanHandle(importType));
    }
}