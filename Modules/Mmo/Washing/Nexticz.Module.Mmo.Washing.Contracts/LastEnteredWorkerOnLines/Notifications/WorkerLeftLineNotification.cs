using MediatR;

namespace Nexticz.Module.Mmo.Washing.Contracts.LastEnteredWorkerOnLines.Notifications;

public record WorkerLeftLineNotification(
    Guid Id, string LineCode, Guid WorkerId, string WorkerName, DateTimeOffset LeftAt) : INotification;