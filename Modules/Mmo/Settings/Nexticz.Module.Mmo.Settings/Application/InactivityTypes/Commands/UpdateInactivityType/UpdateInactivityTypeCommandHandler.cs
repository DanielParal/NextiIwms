using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.Settings.Application.InactivityTypes.Queries.GetInactivityTypeById;
using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Domain.InactivityTypeEntity;
using Nexticz.Module.Mmo.Settings.Domain.InactivityTypeEntity.Events;

namespace Nexticz.Module.Mmo.Settings.Application.InactivityTypes.Commands.UpdateInactivityType;

internal class UpdateInactivityTypeCommandHandler(
    ILogger<UpdateInactivityTypeCommandHandler> logger,
    ISettingsUnitOfWork unitOfWork,
    ISender sender
    ) : IRequestHandler<UpdateInactivityTypeCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(UpdateInactivityTypeCommand request, CancellationToken cancellationToken)
    {
        var inactivityType = await sender.Send(new GetInactivityTypeByIdQuery(request.Id), cancellationToken);

        if (inactivityType.IsError)
        {
            logger.LogWarning("MMO - Settings - Did not find object {ObjectName} with Id: {Id}. Nothing to update", nameof(InactivityType), request.Id);
            return inactivityType.Errors;
        }
        
        var inactivityTypeUpdatedEvent = new InactivityTypeUpdatedEvent(request.Id, request.Name, request.AffectProductivity, request.IsCommentNeededForReview);
        unitOfWork.AppendEvent(inactivityType.Value.Id, inactivityTypeUpdatedEvent);
        
        logger.LogInformation("MMO - Settings - Object {ObjectName} updated. Id: {Id}, Name: {Name}, AffectProductivity: {AffectProductivity}, IsCommentNeededForReview: {IsCommentNeededForReview}", 
            nameof(InactivityType), request.Id, request.Name, request.AffectProductivity, request.IsCommentNeededForReview);
        
        return Result.Success;
    }
}