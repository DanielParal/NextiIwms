using Nexticz.Module.Mmo.Reporting.Contracts.DriedKits;
using Nexticz.Module.Mmo.Reporting.Domain.DriedKitAggregate;

namespace Nexticz.Module.Mmo.Reporting.Presentation.DriedKits;

internal static class DriedKitResponseFactory
{
    public static DriedKitResponse Create(DriedKit driedKit)
    {
        return new DriedKitResponse(
            driedKit.KitIdFromDrying, driedKit.BatchId, driedKit.CompletedKitsCount, driedKit.KitCode, 
            driedKit.LineCode, driedKit.ExpectedDryingTime, driedKit.DryingStarted, driedKit.DryingEnded,
            (DriedKitDestinationContract)driedKit.Destination, driedKit.TransferredToDryingSectionAt);
    }
}