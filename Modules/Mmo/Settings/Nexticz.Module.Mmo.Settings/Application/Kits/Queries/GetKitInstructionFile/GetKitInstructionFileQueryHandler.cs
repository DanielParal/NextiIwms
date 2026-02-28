using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.Settings.Application.FileHandling;

namespace Nexticz.Module.Mmo.Settings.Application.Kits.Queries.GetKitInstructionFile;

internal class GetKitInstructionFileQueryHandler(
    ISettingsFileHandler fileHandler,
    ILogger<GetKitInstructionFileQueryHandler> logger) : IRequestHandler<GetKitInstructionFileQuery, ErrorOr<KitInstructionResult>>
{
    public async Task<ErrorOr<KitInstructionResult>> Handle(GetKitInstructionFileQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.KitCode))
        {
            logger.LogWarning("Settings - Kit code is null or empty. KitCode: {KitCode}", request.KitCode);
            return KitErrors.NotFoundKitInstructionFile;
        }
        
        var fileResult = await fileHandler.GetKitInstructionPdfAsync(request.KitCode, cancellationToken);

        if (fileResult is null)
        {
            logger.LogWarning("Settings - Kit instruction file not found. KitCode: {KitCode}", request.KitCode);
            return KitErrors.NotFoundKitInstructionFile;
        }
        
        return new KitInstructionResult(
            fileResult.ContentBytes, fileResult.ContentType, fileResult.FileName);
    }
}