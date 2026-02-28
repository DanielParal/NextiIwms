using MediatR;

namespace Nexticz.Module.Mmo.Planning.Contracts.WashingMachines.Notifications;

public record SisterBatchDetachedNotification(
    Guid BatchId, Guid SisterBatchId, string WashingMachineCode, 
    string LineCode, string SisterLineCode) : INotification;