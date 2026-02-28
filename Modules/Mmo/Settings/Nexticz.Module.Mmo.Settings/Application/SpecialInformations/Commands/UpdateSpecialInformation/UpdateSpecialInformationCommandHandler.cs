using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.SharedKernel;
using Nexticz.Lib.Shared.Errors;
using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Application.SpecialInformations.Queries.GetSpecialInformationById;
using Nexticz.Module.Mmo.Settings.Application.SpecialInformations.Queries.GetSpecialInformationByTitle;
using Nexticz.Module.Mmo.Settings.Domain.SpecialInformationEntity;
using Nexticz.Module.Mmo.Settings.Domain.SpecialInformationEntity.Events;

namespace Nexticz.Module.Mmo.Settings.Application.SpecialInformations.Commands.UpdateSpecialInformation;

internal class UpdateSpecialInformationCommandHandler(
    ISender sender,
    ILogger<UpdateSpecialInformationCommandHandler> logger,
    ISettingsUnitOfWork unitOfWork
    ) : IRequestHandler<UpdateSpecialInformationCommand, ErrorOr<Updated>>
{
    public async Task<ErrorOr<Updated>> Handle(UpdateSpecialInformationCommand request, CancellationToken cancellationToken)
    {
        var specialInformation = await sender.Send(new GetSpecialInformationByIdQuery(request.Id), cancellationToken);
        if (specialInformation.IsError)
        {
            logger.LogInformation("Did not find object {ObjectName} with id: {Id}. Nothing to update.", nameof(SpecialInformation), request.Id);
            return SpecialInformationErrors.ValidationSpecialInformationWithIdNotFound;
        }
        
        if (await IsTitleUsedInDifferentSpecialInformationAsync(request.Title, specialInformation.Value.Title, cancellationToken))
        {
            logger.LogInformation("Object {ObjectName} with title {Title} already exists. Nothing to update.", 
                nameof(SpecialInformation), request.Title);
            return SpecialInformationErrors.ValidationSpecialInformationWithTitleAlreadyExists;
        }
        
        var specialInformationUpdatedEvent = new SpecialInformationUpdatedEvent(specialInformation.Value.Id, request.Title.Trim(), request.Description);
        unitOfWork.AppendEvent(specialInformation.Value.Id, specialInformationUpdatedEvent);
        return Result.Updated;
    }

    private async Task<bool> IsTitleUsedInDifferentSpecialInformationAsync(string requestTitle, string specialInformationTitle, CancellationToken cancellationToken)
    {
        if (string.Equals(specialInformationTitle, requestTitle.Trim(), StringComparison.InvariantCultureIgnoreCase))
            return false;
        
        var specialInformationWitTitle = await sender.Send(new GetSpecialInformationByTitleQuery(requestTitle), cancellationToken);
        return specialInformationWitTitle.HasValue();
    }
}