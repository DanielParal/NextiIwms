using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Mmo.Reporting.Domain.KitAggregate.Events;

public record KitTimeIntervalChangedEvent(Guid Id, DateTimeOffset StartDate, DateTimeOffset EndDate, 
    TimeSpan RealTimeKitDuration, double KitEfficiency, DateTimeOffset UpdatedAt, string UpdatedBy) : IMartenEvent;