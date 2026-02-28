using ErrorOr;
using Nexticz.Lib.Shared.FileHandling;
using Nexticz.Lib.Shared.FileHandling.Models;
using Nexticz.Module.Cuzk.Domain.ImportAggregate;

namespace Nexticz.Module.Cuzk.Application.FileHandling;

public interface ICuzkFileHandler : IFileHandler
{
    Task<ErrorOr<string>> SaveRequestedFileAsync(Guid importId, ImportType type, Stream fileStream, string originFileName, CancellationToken cancellationToken);
    Task<FileResult?> GetRequestedFileAsync(string fileName, CancellationToken cancellationToken);
    void DeleteRequestedFile(string fileName);
}