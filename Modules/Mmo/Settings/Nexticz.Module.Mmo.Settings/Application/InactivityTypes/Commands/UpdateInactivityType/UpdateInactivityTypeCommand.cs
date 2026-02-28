using ErrorOr;

namespace Nexticz.Module.Mmo.Settings.Application.InactivityTypes.Commands.UpdateInactivityType;

internal record UpdateInactivityTypeCommand
    (Guid Id, string Name, bool AffectProductivity, bool IsCommentNeededForReview) : ISettingsCommand<ErrorOr<Success>>;