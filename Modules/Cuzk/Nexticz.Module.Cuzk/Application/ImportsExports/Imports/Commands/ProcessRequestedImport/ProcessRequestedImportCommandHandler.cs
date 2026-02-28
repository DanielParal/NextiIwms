using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Lib.Shared.ImportsExports.Imports.ImportFiles;
using Nexticz.Lib.Shared.Time;
using Nexticz.Module.Cuzk.Application.FileHandling;
using Nexticz.Module.Cuzk.Application.ImportsExports.Imports.ImportHandlers;
using Nexticz.Module.Cuzk.Application.Interfaces;
using Nexticz.Module.Cuzk.Domain.ImportAggregate.Events;

namespace Nexticz.Module.Cuzk.Application.ImportsExports.Imports.Commands.ProcessRequestedImport;

internal class ProcessRequestedImportCommandHandler (
    ILogger<ProcessRequestedImportCommandHandler> logger,
    ICuzkUnitOfWork unitOfWork,
    ICuzkImportHandlerSelector cuzkImportHandlerSelector,
    IClock clock,
    ICuzkFileHandler fileHandler
    ) : IRequestHandler<ProcessRequestedImportCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(ProcessRequestedImportCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("[Cuzk] [Start] [ProcessRequestedImportCommandHandler]");
            
        var handler = cuzkImportHandlerSelector.GetHandler(request.Import.Type);
        
        if (handler is null)
        {
            logger.LogWarning("[Cuzk] [ProcessRequestedImportCommandHandler] No handler found for {ImportSource}, import id: {ImportId}", 
                request.Import.Type, request.Import.Id);
            return ImportErrors.ValidationHandlerNotImplementedFor(request.Import.Type);
        }
        
        var file = await fileHandler.GetRequestedFileAsync(request.Import.FileName, cancellationToken);
        if (file is null)
        {
            logger.LogWarning("[Cuzk] [ProcessRequestedImportCommandHandler] No file found for {ImportSource}, import id: {ImportId}", 
                request.Import.Type, request.Import.Id);
            return ImportErrors.ValidationFileIsNotFound;
        }
        
        IImportFile byteImportFile = new ByteImportFile(file.ContentBytes, file.FileName, file.ContentType);
        
        var importBaseResult = await handler.HandleAsync(byteImportFile, cancellationToken, request.Import.CsvDelimiter, bulkImport: true);
        
        request.Import.ProcessImport(clock.UtcNowOffset, importBaseResult);
        var requestedImportProcessedEvent = new RequestedImportProcessedEvent(
            request.Import.Id, request.Import.Status, request.Import.Errors, request.Import.DateImported!.Value, request.Import.ImportedCodes);
        
        unitOfWork.AppendEvent(request.Import.Id, requestedImportProcessedEvent);
        
        fileHandler.DeleteRequestedFile(file.FileName);
        
        logger.LogInformation("[Cuzk] [End] [ProcessRequestedImportCommandHandler] data were imported. ImportType: {ImportType}, ImportId: {ImportId}", 
            request.Import.Type, request.Import.Id);
        
        return Result.Success;
    }
}