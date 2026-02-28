using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.Washing.Application.Batches.Queries.GetBatchById;
using Nexticz.Module.Mmo.Washing.Application.FileHandling;
using Nexticz.Module.Mmo.Washing.Domain.BatchAggregate;

namespace Nexticz.Module.Mmo.Washing.Application.Batches.Queries.GetSpecialInformationFile;

internal class GetSpecialInformationFileQueryHandler(
    IWashingFileHandler fileHandler,
    ISender sender,
    ILogger<GetSpecialInformationFileQueryHandler> logger) : IRequestHandler<GetSpecialInformationFileQuery, ErrorOr<SpecialInformationFileResult>>
{
    public async Task<ErrorOr<SpecialInformationFileResult>> Handle(GetSpecialInformationFileQuery request, CancellationToken cancellationToken)
    {
        var batch = await sender.Send(new GetBatchByIdQuery(request.BatchId), cancellationToken);
        if (batch.IsError)
        {
            logger.LogInformation("Washing - {ObjectName} with id {Id} not found. We cannot get special information file.",
                nameof(Batch), request.BatchId);
            return BatchErrors.ValidationBatchDoesNotExist;
        }

        if (batch.Value.SpecialInformation is null)
        {
            logger.LogInformation("Washing - {ObjectName} with id {Id} has no special information. We cannot get special information file.",
                nameof(Batch), request.BatchId);
            return BatchErrors.ValidationBatchDoesNotHaveSpecialInformation;
        }
        
        var fileResult = await fileHandler.GetSpecialInformationFileAsync(batch.Value.SpecialInformation.Id, cancellationToken);

        if (fileResult is null)
        {
            logger.LogWarning("Washing - Special information file not found. BatchId: {BatchId}, Special information id: {SpecialInformationId}", 
                batch.Value.Id, batch.Value.SpecialInformation.Id);
            return BatchErrors.NotFoundSpecialInformationFile;
        }
        
        return new SpecialInformationFileResult(
            fileResult.ContentBytes, fileResult.ContentType, fileResult.FileName);
    }
}