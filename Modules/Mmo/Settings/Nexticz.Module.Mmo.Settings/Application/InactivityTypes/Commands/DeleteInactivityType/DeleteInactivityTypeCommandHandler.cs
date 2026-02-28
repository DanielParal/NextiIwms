using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.Settings.Application.InactivityTypes.Queries.GetInactivityTypeById;
using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Domain.InactivityTypeEntity;
using Nexticz.Module.Mmo.Settings.Domain.InactivityTypeEntity.Events;

namespace Nexticz.Module.Mmo.Settings.Application.InactivityTypes.Commands.DeleteInactivityType;

internal class DeleteInactivityTypeCommandHandler(
    ILogger<DeleteInactivityTypeCommandHandler> logger,
    ISettingsUnitOfWork unitOfWork,
    ISender sender
) : IRequestHandler<DeleteInactivityTypeCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(DeleteInactivityTypeCommand request, CancellationToken cancellationToken)
    {
        var inactivityType = await sender.Send(new GetInactivityTypeByIdQuery(request.Id), cancellationToken);

        if (inactivityType.IsError)
        {
            logger.LogInformation("Settings - Did not find object {ObjectName} with ID: {Id}. Nothing to delete.", 
                nameof(InactivityType), request.Id);
            return inactivityType.Errors;
        }
        
        var inactivityTypeDeletedEvent = new InactivityTypeDeletedEvent(inactivityType.Value.Id);
        unitOfWork.AppendEvent(inactivityType.Value.Id, inactivityTypeDeletedEvent);
        
        logger.LogInformation("Settings - Object {ObjectName} deleted. Id: {Id}.", 
            nameof(InactivityType), request.Id);
        
        return Result.Success;
    }
}