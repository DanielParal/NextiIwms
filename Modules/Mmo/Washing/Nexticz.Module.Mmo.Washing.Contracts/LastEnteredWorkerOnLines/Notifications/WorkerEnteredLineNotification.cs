using MediatR;

namespace Nexticz.Module.Mmo.Washing.Contracts.LastEnteredWorkerOnLines.Notifications;

public record WorkerEnteredLineNotification(
    Guid Id, string LineCode, Guid WorkerId, string WorkerName, DateTimeOffset EnteredAt) : INotification;