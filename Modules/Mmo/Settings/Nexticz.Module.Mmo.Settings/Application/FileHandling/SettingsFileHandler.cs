using ErrorOr;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.SharedKernel.FileHandling;
using Nexticz.Lib.Shared.FileHandling.Assets;

namespace Nexticz.Module.Mmo.Settings.Application.FileHandling;

internal class SettingsFileHandler(AssetsSettings assetsSettings, ILogger<SettingsFileHandler> logger)
    : FileHandler(assetsSettings), ISettingsFileHandler
{
    
    private static string GetSpecialInformationFilename(Guid id, string extension) => $"SI_{id}{extension}";
    
    
    public async Task<Success> SaveKitInstructionPdfAsync(IFormFile file, string kitCode, CancellationToken cancellationToken)
    {
        var kitBaseFolder = GetKitBaseFolderPath(kitCode);
        
        if (!Directory.Exists(kitBaseFolder))
            Directory.CreateDirectory(kitBaseFolder);
        
        var kitInstructionFolder = GetKitInstructionFolderPath(kitCode);
        
        if (!Directory.Exists(kitInstructionFolder))
            Directory.CreateDirectory(kitInstructionFolder);
        
        var existingFiles = Directory.GetFiles(kitInstructionFolder);
        foreach (var existingFile in existingFiles)
        {
            File.Delete(existingFile);
        }
        
        var filePath = Path.Combine(kitInstructionFolder, GetKitInstructionFileName(kitCode));
        await using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream, cancellationToken);
        
        return Result.Success;
    }
    
    public Success DeleteKitInstructionPdf(string kitCode)
    {
        var kitInstructionFolder = GetKitInstructionFolderPath(kitCode);
        
        if (!Directory.Exists(kitInstructionFolder))
            return Result.Success;
        
        var existingFiles = Directory.GetFiles(kitInstructionFolder);
        foreach (var existingFile in existingFiles)
        {
            File.Delete(existingFile);
        }
        
        Directory.Delete(kitInstructionFolder);
        
        return Result.Success;
    }

    public async Task<Success> SaveSpecialInformationFileAsync(IFormFile file, Guid id, CancellationToken cancellationToken)
    {
        var specialInformationBaseFolderPath = GetSpecialInformationBaseFolderPath(id);
        
        if (!Directory.Exists(specialInformationBaseFolderPath))
            Directory.CreateDirectory(specialInformationBaseFolderPath);
        
        var specialInformationFolderPath = GetSpecialInformationFolderPath(id);
        
        if (!Directory.Exists(specialInformationFolderPath))
            Directory.CreateDirectory(specialInformationFolderPath);
        
        var existingFiles = Directory.GetFiles(specialInformationFolderPath);
        foreach (var existingFile in existingFiles)
        {
            File.Delete(existingFile);
        }
        var extension = Path.GetExtension(file.FileName);
        var filePath = Path.Combine(specialInformationFolderPath, GetSpecialInformationFilename(id, extension));
        await using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream, cancellationToken);
        
        return Result.Success;
    }

    public Success DeleteSpecialInformationFile(Guid id)
    {
        var specialInformationFolderPath = GetSpecialInformationFolderPath(id);
        
        if (!Directory.Exists(specialInformationFolderPath))
            return Result.Success;
        
        var existingFiles = Directory.GetFiles(specialInformationFolderPath);
        foreach (var existingFile in existingFiles)
        {
            File.Delete(existingFile);
        }
        
        Directory.Delete(specialInformationFolderPath);
        Directory.Delete(GetSpecialInformationBaseFolderPath(id));
        
        return Result.Success;
    }

    public ErrorOr<Success> IsFileValid(IFormFile file)
    {
        if (file.Length == 0)
        {
            logger.LogWarning("Settings - File is empty. Nothing to upload.");
            return FileHandlingErrors.ValidationFileIsEmpty;
        }

        const int maxFileSize = 2 * 1024 * 1024; // 2 MB
        if (file.Length > maxFileSize)
        {
            logger.LogWarning("Settings - File exceeds max file size. Nothing to upload. MaxFileSize: {MaxFileSize}.", maxFileSize);
            return FileHandlingErrors.ValidationFileExceedsMaxSize(2);
        }
        
        return Result.Success;
    }

    public async Task<ErrorOr<IFormFile>> GetFileFromHttpRequestAsync(HttpRequest request, CancellationToken cancellationToken)
    {
        if (!request.HasFormContentType)
        {
            logger.LogWarning("Settings - HttpRequest does not have form content type.");
            return FileHandlingErrors.ValidationMultipartFormRequired;
        }
        
        var form = await request.ReadFormAsync(cancellationToken);
        
        var file = form.Files.GetFile("file");

        if (file is null || file.Length == 0)
        {
            logger.LogWarning("Settings - HttpRequest does not contain a file.");
            return FileHandlingErrors.ValidationNoFileAttached;
        }

        return ErrorOrFactory.From(file);
    }
}