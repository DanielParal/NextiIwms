using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Mmo.Reporting.Domain.InactivityAggregate.Events;

public record InactivityUnplannedEvent(Guid Id, DateTimeOffset UpdatedAt, string UpdatedBy) : IMartenEvent;