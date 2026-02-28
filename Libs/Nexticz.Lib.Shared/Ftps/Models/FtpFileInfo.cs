using FluentFTP;

namespace Nexticz.Lib.Shared.Ftps.Models;

public class FtpFileInfo
{
    public string Name { get; private set; }
    public long Size { get; private set; }
    public FtpFileType Type { get; private set; }
    public DateTime LastModifiedTime { get; private set; }

    public FtpFileInfo(FtpListItem ftpListItem)
    {
        Name = ftpListItem.Name;
        Size = ftpListItem.Size;
        LastModifiedTime = ftpListItem.Modified;

        Type = ftpListItem.Type switch
        {
            FtpObjectType.File => FtpFileType.File,
            FtpObjectType.Directory => FtpFileType.Directory,
            _ => FtpFileType.Link
        };
    }
}