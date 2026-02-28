
using Nexticz.Lib.Shared.FileHandling.Models;

namespace Nexticz.Lib.Shared.ImportsExports.Exports;

public interface IExportXlsxHandler<TExportType>
{
    Task<FileResult> HandleAsync(CancellationToken cancellationToken);
    bool CanHandle(TExportType exportType);
}