using MediatR;
using ErrorOr;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.Washing.Application.FileHandling;

namespace Nexticz.Module.Mmo.Washing.Application.Batches.Queries.GetKitInstructionFile;

internal class GetKitInstructionFileQueryHandler(
    IWashingFileHandler fileHandler,
    ILogger<GetKitInstructionFileQueryHandler> logger) : IRequestHandler<GetKitInstructionFileQuery, ErrorOr<KitInstructionResult>>
{
    public async Task<ErrorOr<KitInstructionResult>> Handle(GetKitInstructionFileQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.KitCode))
        {
            logger.LogWarning("Washing - Kit code is null or empty. KitCode: {KitCode}", request.KitCode);
            return BatchErrors.NotFoundKitInstructionFile;
        }
        
        var fileResult = await fileHandler.GetKitInstructionPdfAsync(request.KitCode, cancellationToken);

        if (fileResult is null)
        {
            logger.LogWarning("Washing - Kit instruction file not found. KitCode: {KitCode}", request.KitCode);
            return BatchErrors.NotFoundKitInstructionFile;
        }
        
        return new KitInstructionResult(
            fileResult.ContentBytes, fileResult.ContentType, fileResult.FileName);
    }
}