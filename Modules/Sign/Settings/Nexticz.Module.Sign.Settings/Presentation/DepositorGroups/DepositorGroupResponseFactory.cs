using Nexticz.Module.Sign.Settings.Contracts.DepositorGroups;
using Nexticz.Module.Sign.Settings.Domain.DepositorGroupAggregate;

namespace Nexticz.Module.Sign.Settings.Presentation.DepositorGroups;

internal static class DepositorGroupResponseFactory
{
    public static DepositorGroupResponse Create(DepositorGroup depositorGroup)
    {
        return new DepositorGroupResponse(depositorGroup.Id, depositorGroup.Code, depositorGroup.Name);
    }
}