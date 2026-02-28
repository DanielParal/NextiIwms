
using ErrorOr;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Nexticz.Lib.Shared.FileHandling;
using Nexticz.Lib.Shared.FileHandling.Assets;
using Nexticz.Lib.Shared.FileHandling.Models;
using Nexticz.Lib.Shared.Logging;
using Nexticz.Lib.Shared.Time;
using Nexticz.Module.Cuzk.Domain.ImportAggregate;

namespace Nexticz.Module.Cuzk.Application.FileHandling;

internal class CuzkFileHandler(
    ILogger<CuzkFileHandler> logger,
    AssetsSettings assetsSettings,
    IClock clock) : FileHandler(logger), ICuzkFileHandler
{
    public async Task<ErrorOr<string>> SaveRequestedFileAsync(Guid importId, ImportType type, Stream fileStream, string originFileName, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("[Cuzk] [Start] [SaveRequestedFileAsync] Saving requested file for import {ImportId}", importId);
            
            var fileExtension = Path.GetExtension(originFileName);
            var fileName = DirectoryNamesProvider.CreateFileName(importId, fileExtension, type, clock);
            await SaveFileAsync(fileStream, DirectoryNamesProvider.RequestedImportsFolder(assetsSettings), fileName, cancellationToken);
            
            logger.LogInformation("[Cuzk] [End] [SaveRequestedFileAsync] Saved file {FileName}", fileName);
            return fileName; 
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[Cuzk] [ERROR] [SaveRequestedFileAsync] Error when saving file. ErrorMessage: {ErrorMessage}", ex.Message);
            return FileHandlingErrors.ErrorSavingRequestedFile(CorrelationIdProvider.Instance.GetInternalId());
        }
    }

    public async Task<FileResult?> GetRequestedFileAsync(string fileName, CancellationToken cancellationToken)
    {
        return await GetFileAsync(DirectoryNamesProvider.RequestedImportsFolder(assetsSettings), fileName, cancellationToken);
    }

    public void DeleteRequestedFile(string fileName)
    {
        DeleteFile(DirectoryNamesProvider.RequestedImportsFolder(assetsSettings), fileName, deleteFolderIfEmpty: false);
    }
}