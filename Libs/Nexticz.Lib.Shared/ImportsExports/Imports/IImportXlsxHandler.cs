
namespace Nexticz.Lib.Shared.ImportsExports.Imports;

public interface IImportXlsxHandler<TImportType>
{
    Task<ImportBaseResult> HandleAsync(object data, CancellationToken cancellationToken);
    bool CanHandle(TImportType exportType);
}