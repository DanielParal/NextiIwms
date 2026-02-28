using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Sign.Settings.Application.Interfaces;
using Nexticz.Module.Sign.Settings.Application.Locations.Queries.GetLocationByCode;
using Nexticz.Module.Sign.Settings.Domain.LocationAggregate;
using Nexticz.Module.Sign.Settings.Domain.LocationAggregate.Events;

namespace Nexticz.Module.Sign.Settings.Application.Locations.Commands.UpdateLocation;

internal class UpdateLocationCommandHandler(
    ILogger<UpdateLocationCommandHandler> logger,
    ISettingsUnitOfWork unitOfWork,
    ISender sender) 
    : IRequestHandler<UpdateLocationCommand, ErrorOr<Updated>>
{
    public async Task<ErrorOr<Updated>> Handle(UpdateLocationCommand request, CancellationToken cancellationToken)
    {
        var location = await sender.Send(new GetLocationByCodeQuery(request.Code), cancellationToken);

        if (location.IsError)
        {
            logger.LogWarning("Sign - Did not find object {ObjectName} with code: {Code}. Nothing to update", 
                nameof(Location), request.Code);
            return LocationErrors.ValidationCodeDoesNotExist;
        }
        
        var locationUpdatedEvent = new LocationUpdatedEvent(location.Value.Id, request.Code, request.Name);
        unitOfWork.AppendEvent(location.Value.Id, locationUpdatedEvent);
        
        logger.LogInformation("Sign - Object {ObjectName} with code: {Code}, name: {Name} updated.",
            nameof(Location), request.Code, request.Name);
        return Result.Updated;
    }
}