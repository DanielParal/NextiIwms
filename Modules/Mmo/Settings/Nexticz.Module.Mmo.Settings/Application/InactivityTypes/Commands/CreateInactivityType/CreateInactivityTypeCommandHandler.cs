using ErrorOr;
using Marten.Linq.SoftDeletes;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.Settings.Application.InactivityTypes.Queries.GetInactivityTypeByName;
using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Domain.InactivityTypeEntity;
using Nexticz.Module.Mmo.Settings.Domain.InactivityTypeEntity.Events;

namespace Nexticz.Module.Mmo.Settings.Application.InactivityTypes.Commands.CreateInactivityType;

internal class CreateInactivityTypeCommandHandler(
    ILogger<CreateInactivityTypeCommandHandler> logger,
    ISettingsUnitOfWork unitOfWork,
    ISender sender
) : IRequestHandler<CreateInactivityTypeCommand, ErrorOr<InactivityType>>
{
    public async Task<ErrorOr<InactivityType>> Handle(CreateInactivityTypeCommand request, CancellationToken cancellationToken)
    {
        var inactivityTypeWithName = await sender.Send(new GetInactivityTypeByNameQuery(request.Name), cancellationToken);

        if (!inactivityTypeWithName.IsError)
        {
            logger.LogWarning("MMO - Settings - Object {ObjectName} with name: {Name} already exists. Nothing to create.", 
                nameof(InactivityType), request.Name);
            return InactivityTypeErrors.ValidationInactivityTypeWithNameAlreadyExist;
        }

        var inactivityType = new InactivityType(request.Name, request.AffectProductivity, request.IsCommentNeededForReview);
        var inactivityTypeCreatedEvent = new InactivityTypeCreatedEvent(
            inactivityType.Id, inactivityType.Name, inactivityType.AffectProductivity, inactivityType.IsCommentNeededForReview);
        
        unitOfWork
            .StartStream<InactivityTypeCreatedEvent, InactivityType>(
                inactivityType.Id, inactivityTypeCreatedEvent);
        
        logger.LogInformation("MMO - Settings - Object {ObjectName} created. Id: {Id}, Name: {Name}, AffectProductivity: {AffectProductivity}, IsCommentNeededForReview: {IsCommentNeededForReview}", 
            nameof(InactivityType), inactivityType.Id, inactivityType.Name, inactivityType.AffectProductivity, inactivityType.IsCommentNeededForReview);
        
        return inactivityType;
    }
}