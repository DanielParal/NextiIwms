using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Mmo.Reporting.Domain.ShiftAggregate.Events;

public record ShiftUnapprovedEvent(
    Guid Id,
    DateTimeOffset UnapprovedAt) : IMartenEvent;