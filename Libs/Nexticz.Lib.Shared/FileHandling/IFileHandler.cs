using ErrorOr;
using Microsoft.AspNetCore.Http;
using Nexticz.Lib.Shared.FileHandling.Models;

namespace Nexticz.Lib.Shared.FileHandling;

public interface IFileHandler
{
    ErrorOr<Success> IsFileValid(IFormFile file, int maxFileSizeBytes = 2 * 1024 * 1024, string[]? allowedExtensions = null);
    ErrorOr<Success> IsFileValid(FileResult file, int maxFileSizeBytes = 2 * 1024 * 1024, string[]? allowedExtensions = null);
    Task<ErrorOr<IFormFile>> GetFileFromHttpRequestAsync(HttpRequest request, CancellationToken cancellationToken, string fileName = "file");

    Task CopyFileFromSourceToDestinationAsync(string fileName, string newFileName, string sourceFolderPath,
        string destinationFolderPath, bool shouldDeleteSourceFile, bool shouldDeleteFolderIfEmpty,
        CancellationToken cancellationToken);
    Task<ErrorOr<FileResult>> CreateZipFromFilesAsync(List<FileResult> files, string zipFileName, CancellationToken cancellationToken);
    Task<byte[]> ReadAllBytesAsync(IFormFile file, CancellationToken cancellationToken);
    string ComputeSha256Hash(byte[] fileBytes);
}