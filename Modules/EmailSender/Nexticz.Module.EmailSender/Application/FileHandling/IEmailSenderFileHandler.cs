
using Nexticz.Lib.Shared.FileHandling.Models;

namespace Nexticz.Module.EmailSender.Application.FileHandling;

public interface IEmailSenderFileHandler
{
    void DeleteAttachments(string folderGuid);
    Task<FileResult[]> GetAttachmentsAsync(string folderGuid, CancellationToken cancellationToken);
}