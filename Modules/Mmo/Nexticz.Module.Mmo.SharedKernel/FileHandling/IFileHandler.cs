namespace Nexticz.Module.Mmo.SharedKernel.FileHandling;

public interface IFileHandler
{
    Task<FileResult?> GetKitInstructionPdfAsync(string kitCode, CancellationToken cancellationToken);
    Task<FileResult?> GetSpecialInformationFileAsync(Guid id, CancellationToken cancellationToken);
}