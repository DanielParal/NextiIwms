using System.Net.Security;
using FluentFTP;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nexticz.Lib.Shared.Ftps.Models;

namespace Nexticz.Lib.Shared.Ftps;

public abstract class FtpHandler(
    IOptions<FtpSettings> ftpSettings,
    ILogger<FtpHandler> logger) : IFtpHandler
{
    private readonly FtpSettings _ftpSettings = ftpSettings.Value;
    
    public async Task<FtpFileInfo[]> ListVerifiedFtpFileInfosAsync(CancellationToken cancellationToken)
    {
        try
        {
            await using var client = CreateClient();
            
            await client.Connect(cancellationToken);
            
            var items = await client.GetListing(_ftpSettings.BasePath, cancellationToken) ?? [];
            
            if (items.Length == 0)
            {
                await client.Disconnect(cancellationToken);
                return [];
            }
            
            // To verify all files are correctly uploaded, we need to check the file size after 3 seconds again to make sure the file is fully downloaded
            await Task.Delay(TimeSpan.FromSeconds(3), cancellationToken);
            
            var itemsToVerify = await client.GetListing(_ftpSettings.BasePath, cancellationToken) ?? [];
            await client.Disconnect(cancellationToken);
            
            var areItemsStable = AreItemsStable(items, itemsToVerify);

            if (!areItemsStable)
                return [];
            
            return items
                .Where(x => x.Type == FtpObjectType.File)
                .Select(x => new FtpFileInfo(x))
                .ToArray();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting file infos from ftp server: {FtpServer} with path: {FtpPath}. Error message: {ErrorMessage}", 
                _ftpSettings.Host, _ftpSettings.BasePath, ex.Message);
            return [];
        }
    }

    private static bool AreItemsStable(FtpListItem[] originalItems, FtpListItem[] itemsToVerify)
    {
        // Stable item = original file size is equal with the new one
        return originalItems.All(originalItem => 
                itemsToVerify.Any(item => item.FullName == originalItem.FullName && 
                                      item.Size == originalItem.Size));
    }
    
    public async Task DownloadFtpFilesAsync(string localDir, string[] fileNames, bool filesShouldBeDeletedFromFtp = false, CancellationToken cancellationToken = default)
    {
        await using var client = CreateClient();
        await client.Connect(cancellationToken);
        
        var filePaths = fileNames.Select(x => $"{_ftpSettings.BasePath}/{x}").ToArray();
            
        var results = await client.DownloadFiles(localDir, filePaths, token: cancellationToken);

        foreach (var result in results)
        {
            if (result.IsSuccess && filesShouldBeDeletedFromFtp)
                await client.DeleteFile(result.RemotePath, cancellationToken);
        }

        await client.Disconnect(cancellationToken);
    }
    
    private AsyncFtpClient CreateClient()
    {
        var client = new AsyncFtpClient(
            _ftpSettings.Host, _ftpSettings.User, _ftpSettings.Password, _ftpSettings.Port,
            new FtpConfig
            {
                DataConnectionType = FtpDataConnectionType.PASV,
                DataConnectionEncryption = true,
                ValidateAnyCertificate = false
            });

        if (_ftpSettings.UseImplicitEncryptionMode)
        {
            client.Config.EncryptionMode = FtpEncryptionMode.Implicit;
        }
            
        
        client.ValidateCertificate += (sender, e) =>
        {
            if (!_ftpSettings.ShouldValidateSslFingerprint)
            {
                e.Accept = true;
                return;
            }
            
            if (_ftpSettings.ShouldValidateSslFingerprint && string.IsNullOrEmpty(_ftpSettings.SslFingerprint))
            {
                logger.LogWarning("FtpHandler - Ssl fingerprint is not set. Ftp connection will not be validated.");
                e.Accept = false;
                return;
            }
            
            var actualThumbprint = e.Certificate?.GetCertHashString().Replace(" ", "").ToUpperInvariant();
            var fingerprintOk = actualThumbprint == _ftpSettings.SslFingerprint;
        
            if (!fingerprintOk)
            {
                logger.LogWarning("FtpHandler - Ssl fingerprint is not valid. Ftp connection will not be validated.");
            }
            
            e.Accept = fingerprintOk;
        };
        
        return client;
    }
}