using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.Settings.Application.FileHandling;
using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Application.SpecialInformations.Queries.GetSpecialInformationById;
using Nexticz.Module.Mmo.Settings.Domain.SpecialInformationEntity;
using Nexticz.Module.Mmo.Settings.Domain.SpecialInformationEntity.Events;

namespace Nexticz.Module.Mmo.Settings.Application.SpecialInformations.Commands.DeleteSpecialInformationFile;

internal class DeleteSpecialInformationFileCommandHandler(
    ISender sender,
    ILogger<DeleteSpecialInformationFileCommandHandler> logger,
    ISettingsUnitOfWork unitOfWork,
    ISettingsFileHandler fileHandler
) : IRequestHandler<DeleteSpecialInformationFileCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(DeleteSpecialInformationFileCommand request, CancellationToken cancellationToken)
    {
        var specialInformation = await sender.Send(new GetSpecialInformationByIdQuery(request.Id), cancellationToken);
        if (specialInformation.IsError)
        {
            logger.LogInformation("Did not find object {ObjectName} with id: {Id}. We cannot delete file.", nameof(SpecialInformation), request.Id);
            return SpecialInformationErrors.ValidationSpecialInformationWithIdNotFound;
        }

        fileHandler.DeleteSpecialInformationFile(request.Id);
        
        var kitInstructionDeletedEvent = new SpecialInformationImageDeletedEvent(request.Id);
        unitOfWork.AppendEvent(specialInformation.Value.Id, kitInstructionDeletedEvent);
        
        return Result.Success;
    }
}