using Microsoft.Extensions.Logging;
using Nexticz.Module.EmailSender.Contracts;
using Nexticz.Lib.Shared.FileHandling;
using Nexticz.Lib.Shared.FileHandling.Assets;
using Nexticz.Lib.Shared.FileHandling.Models;

namespace Nexticz.Module.EmailSender.Application.FileHandling;

internal class EmailSenderFileHandler(
    ILogger<EmailSenderFileHandler> logger,
    AssetsSettings assetsSettings) : FileHandler(logger), IEmailSenderFileHandler
{
    private readonly string _attachmentsRootFolder = EmailSenderDirectoryNameProvider.RootAttachmentsFolder(assetsSettings.BaseFolder);
    
    public void DeleteAttachments(string folderGuid)
    {
        var folderPath = Path.Combine(_attachmentsRootFolder, folderGuid);
        DeleteDirectoryWithAllItsContent(folderPath);
    }

    public async Task<FileResult[]> GetAttachmentsAsync(string folderGuid, CancellationToken cancellationToken)
    {
        var folderPath = Path.Combine(_attachmentsRootFolder, folderGuid);
        if (!Directory.Exists(folderPath))
            return [];
        
        var filesToReturn = new List<FileResult>();
        var existingFiles = Directory.GetFiles(folderPath);
        
        foreach (var existingFile in existingFiles)
        {
            var fileName = Path.GetFileName(existingFile);
            var file = await GetFileAsync(folderPath, fileName, cancellationToken);
            
            if (file is not null)
                filesToReturn.Add(file);
        }
        
        return filesToReturn.ToArray();
    }
}