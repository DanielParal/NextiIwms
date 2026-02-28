using Nexticz.Module.Mmo.Reporting.Contracts.LineItems;
using Nexticz.Module.Mmo.Reporting.Domain.Views;
using Nexticz.Module.Mmo.Reporting.Domain.WashingMachineSpeedAggregate;
using Nexticz.Module.Mmo.SharedKernel.Efficiencies;

namespace Nexticz.Module.Mmo.Reporting.Presentation.Shifts;

internal static class LineItemContractFactory
{
    public static LineItemContract Create(LineItemView lineItem)
    {
        var lineCodeSuffix = lineItem.LineCode[(lineItem.WashingMachineCode.Length + 1)..];
        var hasSister = lineItem.SisterKitId is not null;
        var efficiencyString = lineItem.KitEfficiency is null ? null : EfficiencyFormatter.EfficiencyToPercentageString(lineItem.KitEfficiency.Value);
        
        return new LineItemContract(
            lineItem.Id,
            lineItem.WashingMachineCode,
            lineItem.LineCode,
            lineCodeSuffix,
            lineItem.StartDate,
            lineItem.EndDate,
            lineItem.IsPlanned,
            (LineItemTypeContract)lineItem.Type,
            hasSister,
            lineItem.AffectProductivity,
            lineItem.InactivityReasonId,
            lineItem.Comment,
            lineItem.BatchId,
            lineItem.KitId,
            lineItem.SisterKitId,
            lineItem.KitCode,
            lineItem.PackagingCode,
            lineItem.OptimalPackagingSpeedOnWashingMachine,
            lineItem.OptimalPackagingSpeedOnWashingMachineLevel is null ? null : (SpeedLevelContract)lineItem.OptimalPackagingSpeedOnWashingMachineLevel,
            lineItem.KitNumber,
            lineItem.KitOrderId,
            lineItem.TotalPlannedKitsCountInBatch,
            lineItem.WashingMachineSpeed,
            lineItem.WashingMachineSpeedLevel is null ? null : (SpeedLevelContract)lineItem.WashingMachineSpeedLevel,
            efficiencyString,
            lineItem.OptimalKitDuration,
            lineItem.DeclaredBy,
            lineItem.UpdatedBy);
    }
    
    public static LineItemContract Create(WashingMachineSpeed speed, string lineCode)
    {
        return new LineItemContract(
            speed.Id,
            speed.Code,
            lineCode,
            lineCode,
            speed.DateStarted,
            speed.DateEnded ?? DateTimeOffset.Now,
            false,
            LineItemTypeContract.Speed,
            false,
            false,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            speed.Speed,
            (SpeedLevelContract)speed.SpeedLevel,
            null,
            null,
            null,
            null);
    }
}