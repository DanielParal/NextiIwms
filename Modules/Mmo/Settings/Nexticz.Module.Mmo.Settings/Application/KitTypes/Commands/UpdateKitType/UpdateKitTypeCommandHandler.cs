using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.SharedKernel;
using Nexticz.Lib.Shared.Errors;
using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Application.KitTypes.Queries.GetKitTypeByCode;
using Nexticz.Module.Mmo.Settings.Domain.KitTypeEntity;
using Nexticz.Module.Mmo.Settings.Domain.KitTypeEntity.Events;

namespace Nexticz.Module.Mmo.Settings.Application.KitTypes.Commands.UpdateKitType;

internal class UpdateKitTypeCommandHandler(
    ILogger<UpdateKitTypeCommandHandler> logger,
    ISettingsUnitOfWork unitOfWork,
    ISender sender
) : IRequestHandler<UpdateKitTypeCommand, ErrorOr<Updated>>
{
    public async Task<ErrorOr<Updated>> Handle(UpdateKitTypeCommand request, CancellationToken cancellationToken)
    {
        var kitType = await sender.Send(new GetKitTypeByCodeQuery(request.Code), cancellationToken);
        
        if (!kitType.HasValue())
        {
            logger.LogInformation("Did not find object {ObjectName} with code: {Code}. Nothing to update.", nameof(KitType), request.Code);
            return KitTypeErrors.CodeDoesNotExist;
        }
        
        var kitTypeNameUpdatedEvent = new KitTypeNameUpdatedEvent(kitType.Value.Id, request.Name);

        unitOfWork.AppendEvent(kitType.Value.Id, kitTypeNameUpdatedEvent);

        return Result.Updated;
    }
}