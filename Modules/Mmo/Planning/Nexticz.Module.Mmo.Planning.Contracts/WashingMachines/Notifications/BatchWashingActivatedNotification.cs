using MediatR;

namespace Nexticz.Module.Mmo.Planning.Contracts.WashingMachines.Notifications;

public record BatchWashingActivatedNotification(
    Guid? FinishedBatchId, Guid? FinishedSisterBatchId,
    int RequestedWashingMachineSpeed,
    SpeedLevelContract RequestedWashingMachineSpeedLevel,
    BatchActivatedResponse ActivatedBatch, BatchActivatedResponse? ActivatedSisterBatch,
    DateTimeOffset DateActivated) : INotification;