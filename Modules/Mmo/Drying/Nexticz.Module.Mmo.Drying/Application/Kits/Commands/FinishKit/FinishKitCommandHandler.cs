using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.Drying.Contracts.Kits;
using Nexticz.Module.Mmo.Drying.Contracts.Kits.Notifications;
using Nexticz.Lib.Shared.Time;
using Nexticz.Module.Mmo.Drying.Application.Interfaces;
using Nexticz.Module.Mmo.Drying.Application.Kits.Queries.GetKitById;
using Nexticz.Module.Mmo.Drying.Application.NotificationCollectors;
using Nexticz.Module.Mmo.Drying.Domain.KitAggregate.Events;

namespace Nexticz.Module.Mmo.Drying.Application.Kits.Commands.FinishKit;

internal class FinishKitCommandHandler(
    ISender sender,
    IClock clock,
    IDryingUnitOfWork unitOfWork,
    IDryingNotificationCollector dryingNotificationCollector,
    ILogger<FinishKitCommandHandler> logger) : IRequestHandler<FinishKitCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(FinishKitCommand request, CancellationToken cancellationToken)
    {
        var kit = await sender.Send(new GetKitByIdQuery(request.KitId), cancellationToken);

        if (kit.IsError)
            return kit.Errors;

        var dryingEnded = clock.UtcNowOffset;
        kit.Value.FinishDrying(dryingEnded);
        
        var kitDryingEndedEvent = new KitFinishedEvent(kit.Value.Id, dryingEnded);
        unitOfWork.AppendEvent(kit.Value.Id, kitDryingEndedEvent);
        
        dryingNotificationCollector.AddNotification(
            new KitDryingFinishedNotification(
                new KitContract(kit.Value.Id, kit.Value.BatchId, kit.Value.GlobalKitsCount, kit.Value.KitCode, kit.Value.LineCode,
                    kit.Value.ExpectedDryingTime, kit.Value.DryingStarted, (DateTimeOffset)kit.Value.DryingEnded!, 
                    (KitDestinationContract)kit.Value.Destination, kit.Value.TransferredToDryingSectionAt)));

        logger.LogInformation("Drying - kit finished drying. Published notification {NotificationName}. KitId: {KitId}.", nameof(KitDryingFinishedNotification), kit.Value.Id);
        
        return Result.Success;
    }
}