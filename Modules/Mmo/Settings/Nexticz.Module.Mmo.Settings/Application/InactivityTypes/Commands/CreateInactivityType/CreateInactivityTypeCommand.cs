using ErrorOr;
using Nexticz.Module.Mmo.Settings.Domain.InactivityTypeEntity;

namespace Nexticz.Module.Mmo.Settings.Application.InactivityTypes.Commands.CreateInactivityType;

internal record CreateInactivityTypeCommand(string Name, bool AffectProductivity, bool IsCommentNeededForReview) : ISettingsCommand<ErrorOr<InactivityType>>;