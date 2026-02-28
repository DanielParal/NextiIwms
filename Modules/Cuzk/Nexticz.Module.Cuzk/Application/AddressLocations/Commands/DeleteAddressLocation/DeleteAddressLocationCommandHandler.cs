using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Lib.Shared.Time;
using Nexticz.Module.Cuzk.Application.AddressLocations.Queries.GetAddressLocationByAdmCode;
using Nexticz.Module.Cuzk.Application.Interfaces;
using Nexticz.Module.Cuzk.Domain.AddressLocationAggregate;
using Nexticz.Module.Cuzk.Domain.AddressLocationAggregate.Events;

namespace Nexticz.Module.Cuzk.Application.AddressLocations.Commands.DeleteAddressLocation;

internal class DeleteAddressLocationCommandHandler(
    ISender sender,
    ILogger<DeleteAddressLocationCommandHandler> logger,
    ICuzkUnitOfWork unitOfWork,
    IClock clock) : IRequestHandler<DeleteAddressLocationCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(DeleteAddressLocationCommand request, CancellationToken cancellationToken)
    {
        var existing = await sender.Send(new GetAddressLocationByAdmCodeQuery(request.AdmCode), cancellationToken);

        if (existing.IsError)
        {
            logger.LogInformation("[Cuzk] [DeleteAddressLocationCommandHandler] Did not find object {ObjectName}. Nothing to delete. Request: {@Request}",
                nameof(AddressLocation), request);
            return AddressLocationErrors.ValidationAddressLocationWithAdmCodeDoesNotExist;
        }

        var deletedEvent = new AddressLocationDeletedEvent(existing.Value.Id, existing.Value.AdmCode, existing.Value.MunicipalityCode, clock.UtcNowOffset);
        unitOfWork.AppendEvent(existing.Value.Id, deletedEvent);

        logger.LogInformation("[Cuzk] [DeleteAddressLocationCommandHandler] Object {ObjectName} with id: {Id} deleted. Request: {@Request}",
            nameof(AddressLocation), existing.Value.Id, request);

        return Result.Success;
    }
}