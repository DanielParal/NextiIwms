using System.IO.Compression;
using System.Security.Cryptography;
using ErrorOr;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Nexticz.Lib.Shared.FileHandling.Models;

namespace Nexticz.Lib.Shared.FileHandling;

public abstract class FileHandler(ILogger<FileHandler> logger) : IFileHandler
{
    public ErrorOr<Success> IsFileValid(IFormFile file, int maxFileSizeBytes = 2 * 1024 * 1024, string[]? allowedExtensions = null)
    {
        return IsFileValid(file.Length, file.FileName, maxFileSizeBytes, allowedExtensions);
    }

    public ErrorOr<Success> IsFileValid(FileResult file, int maxFileSizeBytes = 2097152, string[]? allowedExtensions = null)
    {
        return IsFileValid(file.Length, file.FileName, maxFileSizeBytes, allowedExtensions);
    }
    
    private ErrorOr<Success> IsFileValid(long fileLength, string fileName, int maxFileSizeBytes = 2097152, string[]? allowedExtensions = null)
    {
        if (fileLength == 0)
        {
            logger.LogWarning("[FileHandler] [IsFileValid] File is empty. Nothing to upload.");
            return FileHandlingErrors.ValidationFileIsEmpty;
        }
        
        if (fileLength > maxFileSizeBytes)
        {
            logger.LogWarning("[FileHandler] [IsFileValid] File exceeds max file size. Nothing to upload. MaxFileSize: {MaxFileSize}.", maxFileSizeBytes);
            return FileHandlingErrors.ValidationFileExceedsMaxSize(maxFileSizeBytes / 1024);
        }
        
        if (allowedExtensions is null)
            return Result.Success;
        
        var fileExtension = Path.GetExtension(fileName).ToLowerInvariant();
        var lowerCaseAllowedExtensions = allowedExtensions.Select(x => x.ToLowerInvariant()).ToList();
        
        if (lowerCaseAllowedExtensions.Contains(fileExtension)) 
            return Result.Success;
        
        var joinedAllowedExtensions = string.Join(", ", lowerCaseAllowedExtensions);
        logger.LogWarning("[FileHandler] [IsFileValid] Invalid file type. Allowed types: {AllowedTypes}.", joinedAllowedExtensions);
        return FileHandlingErrors.ValidationInvalidFileType(joinedAllowedExtensions);
    }

    public async Task<ErrorOr<IFormFile>> GetFileFromHttpRequestAsync(HttpRequest request, CancellationToken cancellationToken, string fileName = "file")
    {
        if (!request.HasFormContentType)
        {
            logger.LogWarning("[FileHandler] [IsFileValid] HttpRequest does not have form content type.");
            return FileHandlingErrors.ValidationMultipartFormRequired;
        }
        
        var form = await request.ReadFormAsync(cancellationToken);
        
        var file = form.Files.GetFile(fileName);

        if (file is null || file.Length == 0)
        {
            logger.LogWarning("[FileHandler] [IsFileValid] HttpRequest does not contain a file.");
            return FileHandlingErrors.ValidationNoFileAttached;
        }

        return ErrorOrFactory.From(file);
    }
    
    public async Task CopyFileFromSourceToDestinationAsync(string fileName, string newFileName, string sourceFolderPath, 
        string destinationFolderPath, bool shouldDeleteSourceFile, bool shouldDeleteFolderIfEmpty, CancellationToken cancellationToken)
    {
        if (!Directory.Exists(destinationFolderPath))
            Directory.CreateDirectory(destinationFolderPath);

        var destinationPath = Path.Combine(destinationFolderPath, newFileName);
        var sourcePath = Path.Combine(sourceFolderPath, fileName);

        if (!File.Exists(sourcePath))
        {
            logger.LogError("[Error] [FileHandler] [CopyFileFromSourceToDestinationAsync] error copying file because source file does not exist. SourcePath: {SourcePath}, DestinationPath: {DestinationPath}", sourcePath, destinationFolderPath);
            return;
        }
        
        await using (var sourceStream = File.OpenRead(sourcePath))
        {
            await using (var destinationStream = File.Create(destinationPath))
            {
                await sourceStream.CopyToAsync(destinationStream, cancellationToken);
            }
        }
                
        if (shouldDeleteSourceFile)
            File.Delete(sourcePath);
        
        if (shouldDeleteFolderIfEmpty && Directory.GetFiles(sourceFolderPath).Length == 0 && Directory.GetDirectories(sourceFolderPath).Length == 0)
            Directory.Delete(sourceFolderPath);
    }

    public async Task<ErrorOr<FileResult>> CreateZipFromFilesAsync(List<FileResult> files, string zipFileName, CancellationToken cancellationToken)
    {
        if (files.Count == 0)
            return FileHandlingErrors.ValidationNoFilesProvidedToCreateZip;
        
        if (string.IsNullOrWhiteSpace(zipFileName))
            return FileHandlingErrors.ValidationInvalidZipFileName;

        try
        {
            using var memoryStream = new MemoryStream();
            using (var zip = new ZipArchive(memoryStream, ZipArchiveMode.Create, leaveOpen: true))
            {
                foreach (var file in files)
                {
                    var entry = zip.CreateEntry(file.FileName, CompressionLevel.Fastest);
                    await using var entryStream = entry.Open();
                    await entryStream.WriteAsync(file.ContentBytes.AsMemory(0, file.ContentBytes.Length), cancellationToken);
                }
            }

            memoryStream.Seek(0, SeekOrigin.Begin);
        
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(zipFileName);
            var fileNameWithCorrectExtension = $"{fileNameWithoutExtension}.zip";
            
            return new FileResult(
                contentBytes: memoryStream.ToArray(),
                contentType: "application/zip",
                fileName: fileNameWithCorrectExtension
            );
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[Error] [FileHandler] [CreateZipFromFilesAsync] Error creating zip file. Message: {ErrorMessage}.", ex.Message);
            return FileHandlingErrors.ZipCreationFailed;
        }
    }

    public async Task<byte[]> ReadAllBytesAsync(IFormFile file, CancellationToken cancellationToken)
    {
        if (file.Length == 0) 
            return [];

        var capacity = file.Length > int.MaxValue
            ? throw new InvalidOperationException("File is too large to be loaded into memory.")
            : (int)file.Length;
        
        await using var ms = new MemoryStream(capacity);
        await file.CopyToAsync(ms, cancellationToken);
        return ms.ToArray();
    }

    public string ComputeSha256Hash(byte[] fileBytes)
    {
        return Convert.ToHexString(SHA256.HashData(fileBytes));
    }

    protected static async Task<Success> SaveFileAsync(IFormFile file, string path, string fileName, CancellationToken cancellationToken,
        bool deleteExistingFilesInDirectory = false)
    {
        var fileStream = file.OpenReadStream();
        return await SaveFileAsync(fileStream, path, fileName, cancellationToken, deleteExistingFilesInDirectory);
    }
    
    protected static async Task<Success> SaveFileAsync(Stream fileStream, string path, string fileName, CancellationToken cancellationToken,
        bool deleteExistingFilesInDirectory = false)
    {
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        if (deleteExistingFilesInDirectory)
        {
            var existingFiles = Directory.GetFiles(path);
            foreach (var existingFile in existingFiles)
            {
                File.Delete(existingFile);
            }
        }
        
        var filePath = Path.Combine(path, fileName);
        await using var stream = new FileStream(filePath, FileMode.Create);
        await fileStream.CopyToAsync(stream, cancellationToken);
        
        return Result.Success;
    }

    protected static string RenameFileWithExistingExtension(IFormFile file, string newName)
    {
        var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
        return $"{newName}{fileExtension}";
    }

    protected static Success DeleteFile(string folderPath, string fileName, bool deleteFolderIfEmpty = true)
    {
        if (!Directory.Exists(folderPath))
            return Result.Success;
        
        var existingFiles = Directory.GetFiles(folderPath);
        var existingFile = existingFiles.FirstOrDefault(x => x.Contains(fileName));
        
        if (string.IsNullOrEmpty(existingFile))
            return Result.Success;
        
        File.Delete(existingFile);

        if (deleteFolderIfEmpty && Directory.GetFiles(folderPath).Length == 0 && Directory.GetDirectories(folderPath).Length == 0)
            Directory.Delete(folderPath);
        
        return Result.Success;
    }
    
    protected static Success DeleteDirectoryWithAllItsContent(string folderPath)
    {
        if (!Directory.Exists(folderPath))
            return Result.Success;
        
        Directory.Delete(folderPath, recursive: true);
        
        return Result.Success;
    }

    protected async Task<FileResult?> GetFileAsync(string folderPath, string fileName, CancellationToken cancellationToken)
    {
        if (!Directory.Exists(folderPath))
            return null;
        
        var firstFile = 
            Directory
                .EnumerateFiles(folderPath, $"{fileName}*", SearchOption.TopDirectoryOnly)
                .FirstOrDefault();
        
        if (firstFile == null)
            return null;
        
        var fileBytes = await File.ReadAllBytesAsync(firstFile, cancellationToken);
        
        return new FileResult(fileBytes, GetContentType(firstFile), Path.GetFileName(firstFile));
    }
    
    protected virtual string GetContentType(string filePath)
    {
        var extension = Path.GetExtension(filePath)?.ToLowerInvariant() ?? string.Empty;
        
        return extension switch
        {
            ".jpg" => "image/jpeg",
            ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".pdf" => "application/pdf",
            ".txt" => "text/plain",
            ".zip" => "application/zip",
            _ => "application/octet-stream"
        };
    }
}