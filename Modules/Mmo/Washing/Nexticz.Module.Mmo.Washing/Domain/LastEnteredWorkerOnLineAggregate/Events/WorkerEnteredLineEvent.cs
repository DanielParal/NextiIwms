using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Mmo.Washing.Domain.LastEnteredWorkerOnLineAggregate.Events;

public record WorkerEnteredLineEvent(Guid Id, string LineCode, Guid WorkerId, string WorkerName, DateTimeOffset EnteredAt) : IMartenEvent;