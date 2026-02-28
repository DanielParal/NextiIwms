using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Lib.Shared.Time;
using Nexticz.Module.Mmo.Reporting.Application.Interfaces;
using Nexticz.Module.Mmo.Reporting.Domain.KitAggregate;
using Nexticz.Module.Mmo.Reporting.Domain.KitAggregate.Events;

namespace Nexticz.Module.Mmo.Reporting.Application.Kits.Commands.CreateKit;

internal class CreateKitCommandHandler(
    ILogger<CreateKitCommandHandler> logger,
    IReportingUnitOfWork unitOfWork,
    IClock clock) : IRequestHandler<CreateKitCommand, ErrorOr<Success>>
{
    public Task<ErrorOr<Success>> Handle(CreateKitCommand request, CancellationToken cancellationToken)
    {
        var kit = new Kit(
            request.KitId,
            request.SisterKitId,
            request.ShiftId,
            request.BatchId,
            request.SisterKitId,
            request.KitOrderId,
            request.TotalPlannedKitsCountInBatch,
            request.WashingMachineCode,
            request.LineCode,
            request.WashingMachineSpeed,
            request.WashingMachineSpeedLevel,
            request.KitCode,
            request.KitNumber,
            request.SapBarcode,
            request.KitSapDefinitionCode,
            request.KitSapDefinitionName,
            request.PackagingCode,
            request.OptimalPackagingSpeedOnWashingMachine,
            request.OptimalPackagingSpeedOnWashingMachineLevel,           
            request.DefiningPackagingCode,
            request.RealTimeKitDuration,
            request.OptimalKitDuration,
            request.WashingStarted,
            request.WashingEnded,
            request.WorkerName,
            request.GlobalKitsCount,
            clock.UtcNowOffset,
            null,
            request.SpecialInformation);
        
        var kitWashingFinishedEvent = new KitCreatedEvent(
            kit.Id, kit.KitIdFromWashing, kit.SisterKitIdFromWashing, kit.ShiftId,
            kit.BatchId, kit.SisterBatchId, kit.WashingMachineCode, kit.LineCode, kit.WashingMachineSpeed,
            kit.WashingMachineSpeedLevel, kit.OrderId, kit.TotalPlannedKitsCountInBatch, kit.KitCode, 
            kit.KitNumber, kit.KitSapDefinitionCode, kit.KitSapDefinitionName,
            kit.SapBarcode, kit.PackagingCode, kit.OptimalPackagingSpeedOnWashingMachine, kit.OptimalPackagingSpeedOnWashingMachineLevel,
            kit.DefiningPackagingCode, kit.DeclaredBy,
            kit.GlobalKitsCount, kit.Efficiency, kit.RealTimeKitDuration, kit.OptimalKitDuration, kit.WashingStarted, 
            kit.WashingEnded, kit.CreatedAt, kit.SpecialInformation);
        
        unitOfWork.AppendEvent(kit.Id, kitWashingFinishedEvent);
        
        logger.LogInformation("Reporting - kit created. Id: {Id}, KitIdFromWashing: {KitIdFromWashing}, SisterKitIdFromWashing: {SisterKitIdFromWashing}.",
            kit.Id, kit.KitIdFromWashing, kit.SisterKitIdFromWashing);
        
        return Task.FromResult<ErrorOr<Success>>(Result.Success);
    }
}