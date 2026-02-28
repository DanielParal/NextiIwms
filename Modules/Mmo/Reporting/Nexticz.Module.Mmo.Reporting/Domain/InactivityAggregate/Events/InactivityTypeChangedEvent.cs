using MediatR;
using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Mmo.Reporting.Domain.InactivityAggregate.Events;

public record InactivityTypeChangedEvent(Guid Id, Guid ShiftId, InactivityType Type, Guid? InactivityReasonId, 
    bool IsCommentNeededForReview, bool AffectProductivity, DateTimeOffset UpdatedAt, string UpdatedBy) : IMartenEvent;