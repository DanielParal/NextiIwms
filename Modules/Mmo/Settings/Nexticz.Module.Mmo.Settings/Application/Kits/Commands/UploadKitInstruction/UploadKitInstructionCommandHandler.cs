using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.Settings.Application.FileHandling;
using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Application.Kits.Queries.GetKitByCode;
using Nexticz.Module.Mmo.Settings.Domain.KitAggregate;
using Nexticz.Module.Mmo.Settings.Domain.KitAggregate.Events;

namespace Nexticz.Module.Mmo.Settings.Application.Kits.Commands.UploadKitInstruction;

internal class UploadKitInstructionCommandHandler(
    ISender sender,
    ILogger<UploadKitInstructionCommandHandler> logger,
    ISettingsFileHandler fileHandler,
    ISettingsUnitOfWork unitOfWork) 
    : IRequestHandler<UploadKitInstructionCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(UploadKitInstructionCommand request, CancellationToken cancellationToken)
    {
        var kit = await sender.Send(new GetKitByCodeQuery(request.KitCode), cancellationToken);
        if (kit.IsError)
        {
            logger.LogWarning("Settings - Did not find object {ObjectName} with Code: {Code}. Nothing to upload.", nameof(Kit), request.KitCode);
            return KitErrors.ValidationKitDoesNotExist;
        }
        
        var fileValidationResult = IsFileValid(request.FormFile, request.KitCode);
        if (fileValidationResult.IsError)
            return fileValidationResult.Errors;
        
        await fileHandler.SaveKitInstructionPdfAsync(request.FormFile, request.KitCode, cancellationToken);
        
        var kitInstructionUploadedEvent = new KitInstructionUploadedEvent(kit.Value.Id, kit.Value.Code);
        unitOfWork.AppendEvent(kit.Value.Id, kitInstructionUploadedEvent);
        
        return Result.Success;
    }

    private ErrorOr<Success> IsFileValid(IFormFile file, string kitCode)
    {
        var isFileValid = fileHandler.IsFileValid(file);
        if (isFileValid.IsError)
        {
            logger.LogWarning("Settings - File is not valid. Nothing to upload. KitCode: {KitCode}.", kitCode);
            return isFileValid.Errors;
        }

        if (!IsPdfFile(file))
        {
            logger.LogWarning("Settings - File is not a PDF. Nothing to upload. KitCode: {KitCode}.", kitCode);
            return KitErrors.ValidationFileIsNotPdf;
        }
        
        return Result.Success;
    }
    
    private static bool IsPdfFile(IFormFile file)
    {
        var validExcelContentTypes = new HashSet<string>
        {
            "application/pdf"
        };
        
        return validExcelContentTypes.Contains(file.ContentType);
    }
}