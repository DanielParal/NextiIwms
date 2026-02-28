using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Lib.Shared.Time;
using Nexticz.Module.Mmo.Drying.Application.Interfaces;
using Nexticz.Module.Mmo.Drying.Application.Kits.Queries.GetKitById;
using Nexticz.Module.Mmo.Drying.Domain.KitAggregate.Events;

namespace Nexticz.Module.Mmo.Drying.Application.Kits.Commands.TransferKitToDryingSection;

internal class TransferKitToDryingSectionCommandHandler(
    ISender sender,
    IClock clock,
    IDryingUnitOfWork unitOfWork,
    ILogger<TransferKitToDryingSectionCommandHandler> logger) : IRequestHandler<TransferKitToDryingSectionCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(TransferKitToDryingSectionCommand request, CancellationToken cancellationToken)
    {
        var kit = await sender.Send(new GetKitByIdQuery(request.KitId), cancellationToken);

        if (kit.IsError)
            return kit.Errors;

        var transferredAt = clock.UtcNowOffset;
        kit.Value.TransferToDryingSection(transferredAt);
        
        var kitToDryingSectionTransferredEvent = new KitToDryingSectionTransferredEvent(kit.Value.Id, transferredAt);
        unitOfWork.AppendEvent(kit.Value.Id, kitToDryingSectionTransferredEvent);

        logger.LogInformation("Drying - kit transferred to drying section. KitId: {KitId}, TransferredAt: {TransferredAt}.", 
            kit.Value.Id, transferredAt);
        
        return Result.Success;
    }
}