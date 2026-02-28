using Nexticz.Lib.Shared.ImportsExports.Imports.ImportFiles;

namespace Nexticz.Lib.Shared.ImportsExports.Imports;

public interface IImportCsvHandler<TImportType>
{
    Task<ImportBaseResult> HandleAsync(IImportFile data, CancellationToken cancellationToken, string? csvDelimiter = null, bool bulkImport = false);
    bool CanHandle(TImportType exportType);
}