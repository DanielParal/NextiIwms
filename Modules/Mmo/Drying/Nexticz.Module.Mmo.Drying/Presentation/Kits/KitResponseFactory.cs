using Nexticz.Module.Mmo.Drying.Contracts.Kits;
using Nexticz.Lib.Shared.Extensions;
using Nexticz.Module.Mmo.Drying.Domain.KitAggregate;

namespace Nexticz.Module.Mmo.Drying.Presentation.Kits;

internal static class KitResponseFactory
{
    public static KitResponse Create(Kit kit, DateTimeOffset tenantNow)
    {
        var remainingTime = kit.DryingStarted.AddMinutes(kit.ExpectedDryingTime) - tenantNow;
        var remainingTimeString = remainingTime < TimeSpan.Zero ? TimeSpan.Zero.ToHhMmSsString() : remainingTime.ToHhMmSsString();
        
        return new KitResponse(
            kit.Id,
            kit.GlobalKitsCount,
            kit.KitCode,
            kit.ExpectedDryingTime,
            kit.DryingStarted,
            remainingTimeString,
            (KitDestinationContract)kit.Destination,
            kit.TransferredToDryingSectionAt);
    }
}