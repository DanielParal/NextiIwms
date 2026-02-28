
using MediatR;
using ErrorOr;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.SharedKernel;
using Nexticz.Lib.Shared.Errors;
using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Application.Manufactures.Queries.GetManufactureByCode;
using Nexticz.Module.Mmo.Settings.Domain.ManufactureEntity;
using Nexticz.Module.Mmo.Settings.Domain.ManufactureEntity.Events;

namespace Nexticz.Module.Mmo.Settings.Application.Manufactures.Commands.UpdateManufacture;

internal class UpdateManufactureCommandHandler(
    ILogger<UpdateManufactureCommandHandler> logger,
    ISettingsUnitOfWork unitOfWork,
    ISender sender
) : IRequestHandler<UpdateManufactureCommand, ErrorOr<Updated>>
{
    public async Task<ErrorOr<Updated>> Handle(UpdateManufactureCommand request, CancellationToken cancellationToken)
    {
        var manufacture = await sender.Send(new GetManufactureByCodeQuery(request.Code), cancellationToken);
        
        if (!manufacture.HasValue())
        {
            logger.LogInformation("Did not find object {ObjectName} with code: {Code}. Nothing to update.", nameof(Manufacture), request.Code);
            return ManufactureErrors.CodeDoesNotExist;
        }
        
        var manufactureNameUpdatedEvent = new ManufactureNameUpdatedEvent(manufacture.Value.Id, request.Name);

        unitOfWork.AppendEvent(manufacture.Value.Id, manufactureNameUpdatedEvent);

        return Result.Updated;
    }
}