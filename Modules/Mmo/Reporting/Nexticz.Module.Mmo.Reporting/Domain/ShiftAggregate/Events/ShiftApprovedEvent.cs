using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Mmo.Reporting.Domain.ShiftAggregate.Events;

public record ShiftApprovedEvent(
    Guid Id,
    string ApprovedBy,
    DateTimeOffset ApprovedAt) : IMartenEvent;