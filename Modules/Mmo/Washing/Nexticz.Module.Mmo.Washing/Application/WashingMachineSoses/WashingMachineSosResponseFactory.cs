using Nexticz.Module.Mmo.Washing.Contracts.WashingMachineSoses;
using Nexticz.Module.Mmo.Washing.Domain.WashingMachineSosAggregate;

namespace Nexticz.Module.Mmo.Washing.Application.WashingMachineSoses;

internal static class WashingMachineSosResponseFactory
{
    public static WashingMachineSosResponse Create(WashingMachineSos washingMachineSos)
    {
        return new WashingMachineSosResponse(washingMachineSos.Code, washingMachineSos.IsHelpNeeded);
    }
}