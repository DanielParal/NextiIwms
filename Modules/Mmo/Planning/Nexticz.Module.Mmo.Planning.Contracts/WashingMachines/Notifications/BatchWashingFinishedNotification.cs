using MediatR;

namespace Nexticz.Module.Mmo.Planning.Contracts.WashingMachines.Notifications;

public record BatchWashingFinishedNotification(Guid BatchId, Guid? SisterBatchId, DateTimeOffset DateFinished) : INotification;