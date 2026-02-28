using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.Settings.Contracts.Kits.Queries;
using Nexticz.Module.Mmo.Drying.Application.Interfaces;
using Nexticz.Module.Mmo.Drying.Domain.KitAggregate;
using Nexticz.Module.Mmo.Drying.Domain.KitAggregate.Events;

namespace Nexticz.Module.Mmo.Drying.Application.Kits.Commands.CreateKit;

internal class CreateKitCommandHandler(
    ISender sender,
    IDryingUnitOfWork unitOfWork,
    ILogger<CreateKitCommandHandler> logger) : IRequestHandler<CreateKitCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(CreateKitCommand request, CancellationToken cancellationToken)
    {
        var kitFromSettings = await sender.Send(new GetKitResponseByCodeQuery(request.KitCode), cancellationToken);
        if (kitFromSettings.IsError)
            return kitFromSettings.Errors;
        
        var kit = new Kit(request.BatchId, request.GlobalKitsCount, request.KitCode, request.LineCode, 
            kitFromSettings.Value.DryingTime, request.DateFinished, KitDestination.CompletingSection, request.KitId);
        var createdEvent = new KitCreatedEvent(kit.Id, kit.GlobalKitsCount, kit.BatchId, kit.KitCode, kit.LineCode, kit.ExpectedDryingTime, kit.DryingStarted);
        unitOfWork.StartStream<KitCreatedEvent, Kit>(kit.Id, createdEvent);
        
        logger.LogInformation("Drying - Created kit: {KitId}", kit.Id);
        return Result.Success;
    }
}