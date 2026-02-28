
using ErrorOr;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Sign.SharedKernel.FileHandling;
using Nexticz.Lib.Shared.FileHandling;
using Nexticz.Lib.Shared.FileHandling.Assets;
using Nexticz.Lib.Shared.FileHandling.Models;

namespace Nexticz.Module.Sign.Settings.Application.FileHandling;

internal class SettingsFileHandler(
    ILogger<SettingsFileHandler> logger,
    AssetsSettings assetsSettings) : FileHandler(logger), ISettingsFileHandler
{
    public async Task<Success> UploadUserSignatureAsync(IFormFile formFile, Guid userId, CancellationToken cancellationToken)
    {
        var userFolderPath = DirectoryNamesProvider.GetBaseSignaturePath(assetsSettings) + "/" + userId;
        var fileName = RenameFileWithExistingExtension(formFile, DirectoryNamesProvider.GetUserSignatureFileNameWithoutExtension);
        await SaveFileAsync(formFile, userFolderPath, fileName, cancellationToken);
        return Result.Success;       
    }

    public async Task<FileResult?> GetUserSignatureAsync(Guid userId, CancellationToken cancellationToken)
    {
        var userFolderPath = $"{DirectoryNamesProvider.GetBaseSignaturePath(assetsSettings)}/{userId}";
        return await GetFileAsync(userFolderPath, DirectoryNamesProvider.GetUserSignatureFileNameWithoutExtension, cancellationToken);
    }

    public Success DeleteUserSignature(Guid userId)
    {
        var userFolderPath = $"{DirectoryNamesProvider.GetBaseSignaturePath(assetsSettings)}/{userId}";
        DeleteFile(userFolderPath, DirectoryNamesProvider.GetUserSignatureFileNameWithoutExtension);
        return Result.Success;       
    }
}