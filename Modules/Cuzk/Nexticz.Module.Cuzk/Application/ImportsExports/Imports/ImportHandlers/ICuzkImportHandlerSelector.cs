using Nexticz.Module.Cuzk.Domain.ImportAggregate;

namespace Nexticz.Module.Cuzk.Application.ImportsExports.Imports.ImportHandlers;

internal interface ICuzkImportHandlerSelector
{
    ICuzkImportHandler<ImportType>? GetHandler(ImportType importType);
}