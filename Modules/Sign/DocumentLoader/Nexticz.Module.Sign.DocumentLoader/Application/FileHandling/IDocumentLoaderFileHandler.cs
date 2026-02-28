namespace Nexticz.Module.Sign.DocumentLoader.Application.FileHandling;

public interface IDocumentLoaderFileHandler
{
    Task<string[]> ListVerifiedFtpFilesAsync(CancellationToken cancellationToken);
    string[] GetXmlFilesToLoad(string searchPattern = "*.xml");
    Task CopyFileFromFtpToLoaderFolderAsync(string roundKey, string[] fileNames, CancellationToken cancellationToken);
    bool DoesFileExist(string filePath);
}