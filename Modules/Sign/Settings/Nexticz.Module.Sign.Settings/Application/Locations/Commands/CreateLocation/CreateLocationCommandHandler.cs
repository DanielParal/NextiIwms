using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Sign.SharedKernel;
using Nexticz.Lib.Shared.Errors;
using Nexticz.Module.Sign.Settings.Application.Interfaces;
using Nexticz.Module.Sign.Settings.Application.Locations.Queries.GetLocationByCode;
using Nexticz.Module.Sign.Settings.Domain.LocationAggregate;
using Nexticz.Module.Sign.Settings.Domain.LocationAggregate.Events;

namespace Nexticz.Module.Sign.Settings.Application.Locations.Commands.CreateLocation;

internal class CreateLocationCommandHandler(
        ILogger<CreateLocationCommandHandler> logger,
        ISettingsUnitOfWork unitOfWork,
        ISender sender
    ) : IRequestHandler<CreateLocationCommand, ErrorOr<Location>>
{
    public async Task<ErrorOr<Location>> Handle(CreateLocationCommand request, CancellationToken cancellationToken)
    {
        var existingLocation = await sender.Send(new GetLocationByCodeQuery(request.Code), cancellationToken);

        if (existingLocation.HasValue())
        {
            logger.LogInformation("Sign - Object {ObjectName} with code: {Code} already exists. Nothing to create.",
                nameof(Location), request.Code);
            return LocationErrors.ValidationCodeAlreadyExists;
        }
            
        var location = new Location(request.Code, request.Name);
        var locationCreatedEvent =
            new LocationCreatedEvent(location.Id, location.Code, location.Name);

        unitOfWork.StartStream<LocationCreatedEvent, Location>(location.Id, locationCreatedEvent);
            
        logger.LogInformation("Sign - Object {ObjectName} with code: {Code} created.",
            nameof(Location), request.Code);
        return location;
    }
}