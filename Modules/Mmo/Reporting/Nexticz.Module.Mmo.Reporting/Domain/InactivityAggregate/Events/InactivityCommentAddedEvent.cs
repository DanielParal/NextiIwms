using MediatR;
using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Mmo.Reporting.Domain.InactivityAggregate.Events;

public record InactivityCommentAddedEvent(Guid Id, Guid ShiftId, string Comment, DateTimeOffset UpdatedAt, string UpdatedBy) : IMartenEvent;