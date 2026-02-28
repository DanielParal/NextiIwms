
using MediatR;
using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Mmo.Reporting.Domain.InactivityAggregate.Events;

public record InactivityCreatedEvent(
    Guid Id, Guid ShiftId, string WashingMachineCode, string LineCode, 
    DateTimeOffset StartDate, DateTimeOffset EndDate, Guid? InactivityReasonId, bool IsCommentNeededForReview,
    bool AffectProductivity, InactivityType Type, bool IsPlanned, DateTimeOffset CreatedAt, string? DeclaredBy) : IMartenEvent;