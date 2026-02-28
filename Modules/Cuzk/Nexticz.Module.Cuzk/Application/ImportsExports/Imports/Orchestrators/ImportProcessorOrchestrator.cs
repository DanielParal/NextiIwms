using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Cuzk.Application.ImportsExports.Imports.Commands.ProcessRequestedImport;
using Nexticz.Module.Cuzk.Application.ImportsExports.Imports.Queries.GetRequestedImports;
using Nexticz.Module.Cuzk.Application.MasstransitPublishers;

namespace Nexticz.Module.Cuzk.Application.ImportsExports.Imports.Orchestrators;

internal class ImportProcessorOrchestrator(
    ISender sender,
    ILogger<ImportProcessorOrchestrator> logger,
    IMasstransitPublisher masstransitPublisher) : IImportProcessorOrchestrator
{
    public async Task ProcessImportsAsync(string roundKey, CancellationToken cancellationToken)
    {
        var requestedImports = await sender.Send(new GetRequestedImportsQuery(), cancellationToken);

        if (requestedImports.Length == 0)
            return;
        
        logger.LogInformation("[Cuzk] [Start] [ProcessImportsAsync] Found {NumberOfRequestedImports} imports to process. RoundKey: {RoundKey}", requestedImports.Length, roundKey);

        var successfullyImportedFiles = 0;
        foreach (var requestedImport in requestedImports)
        {
            var processRequestedResult = await sender.Send(new ProcessRequestedImportCommand(requestedImport, roundKey), cancellationToken);
            if (processRequestedResult.IsError)
            {
                logger.LogWarning("[Cuzk] [ProcessImportsAsync] Requested import {ImportId} has errors to process. ErrorCode: {ErrorCode}, ErrorDescription: {ErrorDescription}. RoundKey: {RoundKey}", 
                    requestedImport.Id, roundKey, processRequestedResult.FirstError.Code, processRequestedResult.FirstError.Description);
                
                await masstransitPublisher.NotifyImportFailedAsync(requestedImport.Id, requestedImport.Type,
                    requestedImport.UserName, processRequestedResult.FirstError.Description, roundKey, cancellationToken);
                
                return;
            }

            await masstransitPublisher.NotifyImportFinishedAsync(requestedImport.Id, requestedImport.Type,
                requestedImport.UserName, cancellationToken);
            
            successfullyImportedFiles++;
            
            logger.LogInformation("[Cuzk] [ProcessImportsAsync] Import {ImportId} was processed. RoundKey: {RoundKey}, Count: {X}/{Z}", 
                requestedImport.Id, roundKey, successfullyImportedFiles, requestedImports.Length);
        }
        
        logger.LogInformation("[Cuzk] [End] [ProcessImportsAsync] {SuccessfullyImportedFiles} file(s) was/were imported. RoundKey: {RoundKey}", successfullyImportedFiles, roundKey);
    }
}