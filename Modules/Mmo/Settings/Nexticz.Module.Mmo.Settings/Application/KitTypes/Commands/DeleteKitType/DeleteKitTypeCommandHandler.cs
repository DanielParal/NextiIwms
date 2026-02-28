using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Application.Kits.Queries.GetKitsByKitTypeCode;
using Nexticz.Module.Mmo.Settings.Application.KitTypes.Queries.GetKitTypeByCode;
using Nexticz.Module.Mmo.Settings.Domain.KitTypeEntity;
using Nexticz.Module.Mmo.Settings.Domain.KitTypeEntity.Events;

namespace Nexticz.Module.Mmo.Settings.Application.KitTypes.Commands.DeleteKitType;

internal class DeleteKitTypeCommandHandler(
    ILogger<DeleteKitTypeCommandHandler> logger,
    ISettingsUnitOfWork unitOfWork,
    ISender sender
) : IRequestHandler<DeleteKitTypeCommand, ErrorOr<Deleted>>
{
    public async Task<ErrorOr<Deleted>> Handle(DeleteKitTypeCommand request, CancellationToken cancellationToken)
    {
        var kitType = await sender.Send(new GetKitTypeByCodeQuery(request.Code), cancellationToken);

        if (kitType.IsError)
        {
            logger.LogInformation("Did not find object {ObjectName} with code: {Code}. Nothing to delete.", nameof(KitType), request.Code);
            return kitType.Errors;
        }

        var existingKitsWithKitTypeCode = await sender.Send(new GetKitsByKitTypeCodeQuery(request.Code), cancellationToken);

        if (existingKitsWithKitTypeCode.Any())
        {
            var codes = string.Join(", ", existingKitsWithKitTypeCode.Select(x => x.Code));
            logger.LogInformation("{ObjectName} is still used inside kits with codes: {Codes}", nameof(KitType), codes);
            return KitTypeErrors.ValidationCodeIsStillUsedInKits(codes);
        }

        var kitTypeDeletedEvent = new KitTypeDeletedEvent(kitType.Value.Id, request.Code);

        unitOfWork.AppendEvent(kitType.Value.Id, kitTypeDeletedEvent);

        return Result.Deleted;
    }
}