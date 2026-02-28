using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Mmo.Reporting.Domain.WorkerLinePresenceAggregate.Events;

public record WorkerEnteredLineEvent(Guid Id, Guid ShiftId, string LineCode, Guid WorkerId, string WorkerName, DateTimeOffset EnteredAt) : IMartenEvent;