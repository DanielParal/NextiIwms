using Nexticz.Module.Cuzk.Domain.ImportAggregate;

namespace Nexticz.Module.Cuzk.Application.ImportsExports.Imports.ImportHandlers;

internal class CuzkImportHandlerSelector(IEnumerable<ICuzkImportHandler<ImportType>> handlers) : ICuzkImportHandlerSelector
{
    public ICuzkImportHandler<ImportType>? GetHandler(ImportType exportType)
    {
        return handlers.FirstOrDefault(x => x.CanHandle(exportType));
    }
}