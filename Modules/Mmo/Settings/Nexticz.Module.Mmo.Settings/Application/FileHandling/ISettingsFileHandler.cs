using ErrorOr;
using Microsoft.AspNetCore.Http;
using Nexticz.Module.Mmo.SharedKernel.FileHandling;

namespace Nexticz.Module.Mmo.Settings.Application.FileHandling;

internal interface ISettingsFileHandler : IFileHandler
{
    Task<Success> SaveKitInstructionPdfAsync(IFormFile file, string kitCode, CancellationToken cancellationToken);
    Success DeleteKitInstructionPdf(string kitCode);
    Task<Success> SaveSpecialInformationFileAsync(IFormFile file, Guid id, CancellationToken cancellationToken);
    Success DeleteSpecialInformationFile(Guid id);
    ErrorOr<Success> IsFileValid(IFormFile file);
    Task<ErrorOr<IFormFile>> GetFileFromHttpRequestAsync(HttpRequest request, CancellationToken cancellationToken);
}