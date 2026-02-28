using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Mmo.Settings.Domain.InactivityTypeEntity.Events;

public record InactivityTypeCreatedEvent(
    Guid Id, string Name, bool AffectProductivity, bool IsCommentNeededForReview) : IMartenEvent;