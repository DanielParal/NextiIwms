using System.IO.Compression;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Nexticz.Lib.Shared.ImportsExports.Imports.ImportFiles;
using Nexticz.Lib.Shared.Time;
using Nexticz.Lib.Shared.UserProviders;
using Nexticz.Module.Cuzk.Application.FileHandling;
using Nexticz.Module.Cuzk.Application.Interfaces;
using Nexticz.Module.Cuzk.Application.Municipalities.Queries.GetMunicipalitiesWhichShouldBeImported;
using Nexticz.Module.Cuzk.Domain.ImportAggregate;
using Nexticz.Module.Cuzk.Domain.ImportAggregate.Events;

namespace Nexticz.Module.Cuzk.Application.ImportsExports.Imports.Commands.RequestImport;

internal class RequestImportCommandHandler (
    ILogger<RequestImportCommand> logger,
    ISender sender,
    ICuzkUnitOfWork unitOfWork,
    IClock clock,
    ICurrentUserProvider currentUserProvider,
    ICuzkFileHandler fileHandler
    ) : IRequestHandler<RequestImportCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(RequestImportCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("[Cuzk] [Start] [RequestImportCommandHandler] - Requesting import of type {ImportType}", request.ImportType);
        var userName = currentUserProvider.GetCurrentUser().UserName;

        var filesToBeProcessed = request.ImportType == ImportType.AddressLocationZip 
            ? await GetFilesFromZipAsync(request.FormFile, cancellationToken) 
            : [new FormImportFile(request.FormFile)];
        
        var importType = request.ImportType == ImportType.AddressLocationZip ? ImportType.AddressLocationCsv : request.ImportType;

        foreach (var fileToBeProcessed in filesToBeProcessed)
        {
            var importId = Guid.NewGuid();
            var fileName = await fileHandler.SaveRequestedFileAsync(
                importId, importType, fileToBeProcessed.Stream, fileToBeProcessed.FileName, cancellationToken);

            if (fileName.IsError)
            {
                logger.LogWarning("[Cuzk] [End] [RequestImportCommandHandler] Error when saving requested import. ErrorCode: {ErrorCode}", fileName.FirstError.Code);
                return fileName.Errors;
            }
        
            var import = Import.CreateFrom(importId, userName, importType, fileName.Value, request.CsvDelimiter, clock.UtcNowOffset);
            var importRequestedEvent = new ImportRequestedEvent(import.Id, import.UserName, import.Type, import.FileName, import.CsvDelimiter,  import.DateRequested);
        
            unitOfWork.StartStream<ImportRequestedEvent, Import>(import.Id, importRequestedEvent);
        }
        
        logger.LogInformation("[Cuzk] [End] [RequestImportCommandHandler]");
        return Result.Success;
    }
    
    private async Task<IImportFile[]> GetFilesFromZipAsync(IFormFile zipFile, CancellationToken cancellationToken)
    {
        await using var zipStream = zipFile.OpenReadStream();
        using var archive = new ZipArchive(zipStream, ZipArchiveMode.Read, leaveOpen: false);
        
        var municipalitiesWhichShouldBeImported = await sender.Send(new GetMunicipalitiesWhichShouldBeImportedQuery(), cancellationToken);

        var csvEntries = new List<IImportFile>();
        foreach (var municipalityToImport in municipalitiesWhichShouldBeImported)
        {
            var csvEntry = archive.Entries.FirstOrDefault(x => x.Name.Contains(municipalityToImport.Code));
            if (csvEntry != null)
            {
                csvEntries.Add(new ZipImportFile(csvEntry));
            }
        }

        return csvEntries.ToArray();
    } 
    
}