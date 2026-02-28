using MediatR;
using ErrorOr;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Application.Kits.Queries.GetKitsByManufactureCode;
using Nexticz.Module.Mmo.Settings.Application.Manufactures.Queries.GetManufactureByCode;
using Nexticz.Module.Mmo.Settings.Domain.ManufactureEntity;
using Nexticz.Module.Mmo.Settings.Domain.ManufactureEntity.Events;

namespace Nexticz.Module.Mmo.Settings.Application.Manufactures.Commands.DeleteManufacture;

internal class DeleteManufactureCommandHandler(
    ILogger<DeleteManufactureCommandHandler> logger,
    ISettingsUnitOfWork unitOfWork,
    ISender sender
) : IRequestHandler<DeleteManufactureCommand, ErrorOr<Deleted>>
{
    public async Task<ErrorOr<Deleted>> Handle(DeleteManufactureCommand request, CancellationToken cancellationToken)
    {
        var manufacture = await sender.Send(new GetManufactureByCodeQuery(request.Code), cancellationToken);

        if (manufacture.IsError)
        {
            logger.LogInformation("Did not find object {ObjectName} with code: {Code}. Nothing to delete.", nameof(Manufacture), request.Code);
            return manufacture.Errors;
        }
        
        var existingManufactures = await sender.Send(new GetKitsByManufactureCodeQuery(request.Code), cancellationToken);

        if (existingManufactures.Any())
        {
            var codes = string.Join(", ", existingManufactures.Select(x => x.Code));
            logger.LogInformation("{ObjectName} is still used inside kits with codes: {Codes}", nameof(Manufacture), codes);
            return ManufactureErrors.ValidationCodeIsStillUsedInKits(codes);
        }

        var manufactureDeletedEvent = new ManufactureDeletedEvent(manufacture.Value.Id, request.Code);

        unitOfWork.AppendEvent(manufacture.Value.Id, manufactureDeletedEvent);

        return Result.Deleted;
    }
}