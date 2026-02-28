using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Sign.Settings.Application.Interfaces;
using Nexticz.Module.Sign.Settings.Application.Locations.Queries.GetLocationByCode;
using Nexticz.Module.Sign.Settings.Application.SigningDevices.Queries.GetSigningDevicesByLocationCode;
using Nexticz.Module.Sign.Settings.Domain.LocationAggregate;
using Nexticz.Module.Sign.Settings.Domain.LocationAggregate.Events;

namespace Nexticz.Module.Sign.Settings.Application.Locations.Commands.DeleteLocation;

internal class DeleteLocationCommandHandler(
    ILogger<DeleteLocationCommandHandler> logger,
    ISender sender,
    ISettingsUnitOfWork unitOfWork
) 
    : IRequestHandler<DeleteLocationCommand, ErrorOr<Deleted>>
{
    public async Task<ErrorOr<Deleted>> Handle(DeleteLocationCommand request, CancellationToken cancellationToken)
    {
        var location = await sender.Send(new GetLocationByCodeQuery(request.Code), cancellationToken);

        if (location.IsError)
        {
            logger.LogInformation("Sign - Did not find object {ObjectName} with code: {Code}. Nothing to delete.", nameof(Location), request.Code);
            return LocationErrors.ValidationCodeDoesNotExist;
        }
        
        var singingDevices = await sender.Send(new GetSigningDevicesByLocationCodeQuery(location.Value.Code), cancellationToken);
        if (singingDevices.Length > 0)
        {
            var signingDevicesCodes = string.Join(", ", singingDevices.Select(x => x.Code));
            logger.LogInformation("Sign - we cannot delete {ObjectName} with code: {Code} because it is used by signing devices: {SigningDevicesCodes}.",
                nameof(Location), location.Value.Code, signingDevicesCodes);
            return LocationErrors.ValidationCodeIsUsedInSigningDevices(signingDevicesCodes);
        }
        
        var locationDeletedEvent = new LocationDeletedEvent(location.Value.Id, location.Value.Code);
        unitOfWork.AppendEvent(location.Value.Id, locationDeletedEvent);
        
        logger.LogInformation("Sign - Object {ObjectName} with code: {Code} deleted.",
            nameof(Location), location.Value.Code);
        return Result.Deleted;
    }
}