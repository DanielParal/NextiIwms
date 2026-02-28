using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.Settings.Application.FileHandling;
using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Application.SpecialInformations.Queries.GetSpecialInformationById;
using Nexticz.Module.Mmo.Settings.Domain.SpecialInformationEntity;
using Nexticz.Module.Mmo.Settings.Domain.SpecialInformationEntity.Events;

namespace Nexticz.Module.Mmo.Settings.Application.SpecialInformations.Commands.UploadSpecialInformationFile;

internal class UploadSpecialInformationFileCommandHandler(
    ISender sender,
    ILogger<UploadSpecialInformationFileCommandHandler> logger,
    ISettingsUnitOfWork unitOfWork,
    ISettingsFileHandler fileHandler
) : IRequestHandler<UploadSpecialInformationFileCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(UploadSpecialInformationFileCommand request, CancellationToken cancellationToken)
    {
        var specialInformation = await sender.Send(new GetSpecialInformationByIdQuery(request.Id), cancellationToken);
        if (specialInformation.IsError)
        {
            logger.LogInformation("Did not find object {ObjectName} with id: {Id}. We cannot upload file.", nameof(SpecialInformation), request.Id);
            return SpecialInformationErrors.ValidationSpecialInformationWithIdNotFound;
        }
        
        var fileValidationResult = fileHandler.IsFileValid(request.FormFile);
        if (fileValidationResult.IsError)
        {
            logger.LogWarning("Settings - File is not valid. Nothing to upload. Id: {Id}.", request.Id);
            return fileValidationResult.Errors;
        }
        
        await fileHandler.SaveSpecialInformationFileAsync(request.FormFile, request.Id, cancellationToken);
        
        var specialInformationImageUploadedEvent = new SpecialInformationImageUploadedEvent(specialInformation.Value.Id);
        unitOfWork.AppendEvent(specialInformation.Value.Id, specialInformationImageUploadedEvent);
        
        return Result.Success;
    }
}