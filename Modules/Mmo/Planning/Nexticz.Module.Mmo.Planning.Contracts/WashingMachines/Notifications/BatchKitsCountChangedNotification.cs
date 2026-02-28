using MediatR;

namespace Nexticz.Module.Mmo.Planning.Contracts.WashingMachines.Notifications;

public record BatchKitsCountChangedNotification(Guid BatchId, Guid? SisterBatchId, int NewKitsCount) : INotification;