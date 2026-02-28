using Nexticz.Module.Mmo.Settings.Contracts.InactivityTypes;
using Nexticz.Module.Mmo.Settings.Domain.InactivityTypeEntity;

namespace Nexticz.Module.Mmo.Settings.Application.InactivityTypes;

internal static class InactivityTypeResponseFactory
{
    public static InactivityTypeResponse Create(InactivityType inactivityType)
    {
        return new InactivityTypeResponse(inactivityType.Id, inactivityType.Name, inactivityType.AffectProductivity, inactivityType.IsCommentNeededForReview);
    }
}