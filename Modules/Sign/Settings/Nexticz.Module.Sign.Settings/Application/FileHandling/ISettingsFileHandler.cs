using ErrorOr;
using Microsoft.AspNetCore.Http;
using Nexticz.Lib.Shared.FileHandling;
using Nexticz.Lib.Shared.FileHandling.Models;

namespace Nexticz.Module.Sign.Settings.Application.FileHandling;

public interface ISettingsFileHandler : IFileHandler
{
    Task<Success> UploadUserSignatureAsync(IFormFile formFile, Guid userId, CancellationToken cancellationToken);
    Task<FileResult?> GetUserSignatureAsync(Guid userId, CancellationToken cancellationToken);
    Success DeleteUserSignature(Guid userId);
}