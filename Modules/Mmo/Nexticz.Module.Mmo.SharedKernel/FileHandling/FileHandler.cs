using Microsoft.Extensions.Configuration;
using Nexticz.Lib.Shared.FileHandling.Assets;

namespace Nexticz.Module.Mmo.SharedKernel.FileHandling;

public class FileHandler(AssetsSettings assetsSettings) : IFileHandler
{
    private readonly string _baseFolder = assetsSettings.BaseFolder;
    
    private const string KitInstructionFolder = "KitInstruction";
    protected string GetKitBaseFolderPath(string kitCode) => $"{_baseFolder}/MMO/Kits/{kitCode}";
    protected string GetKitInstructionFolderPath(string kitCode) => $"{GetKitBaseFolderPath(kitCode)}/{KitInstructionFolder}";
    protected static string GetKitInstructionFileName(string kitCode) => $"KitInstruction_{kitCode}.pdf";
    
    private const string SpecialInformationFolder = "SpecialInformation";
    protected string GetSpecialInformationBaseFolderPath(Guid id) => $"{_baseFolder}/MMO/SpecialInformations/{id}";
    protected string GetSpecialInformationFolderPath(Guid id) => $"{GetSpecialInformationBaseFolderPath(id)}/{SpecialInformationFolder}";
    
    public async Task<FileResult?> GetKitInstructionPdfAsync(string kitCode, CancellationToken cancellationToken)
    {
        var kitInstructionFolderPath = GetKitInstructionFolderPath(kitCode);

        if (!Directory.Exists(kitInstructionFolderPath))
            return null;
        
        var pdfFile = Directory.EnumerateFiles(kitInstructionFolderPath, "*.pdf", SearchOption.TopDirectoryOnly)
            .FirstOrDefault(file => string.Equals(Path.GetExtension(file), ".pdf", StringComparison.OrdinalIgnoreCase));

        if (pdfFile == null)
            return null;

        var fileBytes = await File.ReadAllBytesAsync(pdfFile, cancellationToken);

        return new FileResult(fileBytes, "application/pdf", GetKitInstructionFileName(kitCode));
    }
    
    public async Task<FileResult?> GetSpecialInformationFileAsync(Guid id, CancellationToken cancellationToken)
    {
        var specialInformationFolderPath = GetSpecialInformationFolderPath(id);

        if (!Directory.Exists(specialInformationFolderPath))
            return null;
        
        var firstFile = 
            Directory
                .EnumerateFiles(specialInformationFolderPath, "*", SearchOption.TopDirectoryOnly)
                .FirstOrDefault();
        
        if (firstFile == null)
            return null;
        
        var fileBytes = await File.ReadAllBytesAsync(firstFile, cancellationToken);
        
        return new FileResult(fileBytes, "application/octet-stream", Path.GetFileName(firstFile));
    }
}