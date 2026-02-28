using Microsoft.Extensions.Logging;
using Nexticz.Module.Sign.SharedKernel.FileHandling;
using Nexticz.Lib.Shared.FileHandling.Assets;
using Nexticz.Module.Sign.DocumentLoader.Application.Interfaces;

namespace Nexticz.Module.Sign.DocumentLoader.Application.FileHandling;

internal class DocumentLoaderFileHandler(
    ILogger<DocumentLoaderFileHandler> logger,
    AssetsSettings assetsSettings,
    IDocumentLoaderFtpHandler documentLoaderFtpHandler) : IDocumentLoaderFileHandler
{
    public async Task<string[]> ListVerifiedFtpFilesAsync(CancellationToken cancellationToken)
    {
        var ftpFileInfos = await documentLoaderFtpHandler.ListVerifiedFtpFileInfosAsync(cancellationToken);
        return ftpFileInfos.Select(x => x.Name).ToArray();
    }

    public string[] GetXmlFilesToLoad(string searchPattern = "*.xml")
    {
        var documentLoaderPath = DirectoryNamesProvider.GetBaseLoaderPath(assetsSettings);
        Directory.CreateDirectory(documentLoaderPath);
        return Directory.GetFiles(documentLoaderPath, searchPattern);
    }

    public async Task CopyFileFromFtpToLoaderFolderAsync(string roundKey, string[] fileNames, CancellationToken cancellationToken)
    {
        var localFolderPath = DirectoryNamesProvider.GetBaseLoaderPath(assetsSettings);
        Directory.CreateDirectory(localFolderPath);
        
        try
        {
            await documentLoaderFtpHandler.DownloadFtpFilesAsync(localFolderPath, fileNames, true, cancellationToken);
                
            logger.LogInformation("SIGN - RoundKey: {RoundKey} - Copied number of files: {FilesCount} to {DestinationPath}", 
                roundKey, fileNames.Length, localFolderPath);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "SIGN - RoundKey: {RoundKey} - Error copying files from FTP server to {LocalPath}", 
                roundKey, localFolderPath);
            throw;
        }
    }

    public bool DoesFileExist(string fileName)
    {
        var documentLoaderPath = DirectoryNamesProvider.GetBaseLoaderPath(assetsSettings);
        var filePath = Path.Combine(documentLoaderPath, fileName);
        return File.Exists(filePath);
    }
}