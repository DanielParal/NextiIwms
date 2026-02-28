using Nexticz.Lib.Shared.Ftps.Models;

namespace Nexticz.Lib.Shared.Ftps;

public interface IFtpHandler
{
    Task<FtpFileInfo[]> ListVerifiedFtpFileInfosAsync(CancellationToken cancellationToken);
    Task DownloadFtpFilesAsync(string localDir, string[] fileNames, bool filesShouldBeDeletedFromFtp = false, CancellationToken cancellationToken = default);
}