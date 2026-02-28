using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.Settings.Application.FileHandling;
using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Application.Kits.Queries.GetKitsBySpecialInformationId;
using Nexticz.Module.Mmo.Settings.Application.SpecialInformations.Queries.GetSpecialInformationById;
using Nexticz.Module.Mmo.Settings.Domain.SpecialInformationEntity;
using Nexticz.Module.Mmo.Settings.Domain.SpecialInformationEntity.Events;

namespace Nexticz.Module.Mmo.Settings.Application.SpecialInformations.Commands.DeleteSpecialInformation;

internal class DeleteSpecialInformationCommandHandler(
    ISender sender,
    ILogger<DeleteSpecialInformationCommandHandler> logger,
    ISettingsUnitOfWork unitOfWork,
    ISettingsFileHandler fileHandler
    ) : IRequestHandler<DeleteSpecialInformationCommand, ErrorOr<Deleted>>
{
    public async Task<ErrorOr<Deleted>> Handle(DeleteSpecialInformationCommand request, CancellationToken cancellationToken)
    {
        var specialInformation = await sender.Send(new GetSpecialInformationByIdQuery(request.Id), cancellationToken);
        if (specialInformation.IsError)
        {
            logger.LogInformation("Did not find object {ObjectName} with id: {Id}. Nothing to delete.", nameof(SpecialInformation), request.Id);
            return SpecialInformationErrors.ValidationSpecialInformationWithIdNotFound;
        }
        
        var kitsWithSpecialInformation = await sender.Send(new GetKitsBySpecialInformationIdQuery(specialInformation.Value.Id), cancellationToken);
        if (kitsWithSpecialInformation.Length > 0)
        {
            var kitIds = string.Join(", ", kitsWithSpecialInformation.Select(x => x.Id));
            logger.LogInformation("{ObjectName} is still used in kits with ids: {KitIds}. Id: {Id}. Nothing to delete.", 
                nameof(SpecialInformation), kitIds, request.Id);
            return SpecialInformationErrors.ValidationSpecialInformationIsStillInUseInKits(kitIds);
        }
        
        fileHandler.DeleteSpecialInformationFile(request.Id);
        
        var specialInformationDeletedEvent = new SpecialInformationDeletedEvent(specialInformation.Value.Id);
        unitOfWork.AppendEvent(specialInformation.Value.Id, specialInformationDeletedEvent);
        return Result.Deleted;
    }
}