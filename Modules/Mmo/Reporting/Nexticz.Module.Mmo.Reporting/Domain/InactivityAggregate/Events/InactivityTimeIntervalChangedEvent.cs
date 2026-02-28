using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Mmo.Reporting.Domain.InactivityAggregate.Events;

public record InactivityTimeIntervalChangedEvent(Guid Id, DateTimeOffset StartDate, DateTimeOffset EndDate, DateTimeOffset UpdatedAt, string UpdatedBy) : IMartenEvent;