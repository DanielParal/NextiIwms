using Nexticz.Module.Mmo.Reporting.Contracts.Shifts;
using Nexticz.Module.Mmo.SharedKernel.Efficiencies;
using Nexticz.Lib.Shared.Extensions;
using Nexticz.Module.Mmo.Reporting.Application.Shifts.Models;

namespace Nexticz.Module.Mmo.Reporting.Presentation.Shifts;

internal static class ShiftSummaryResponseFactory
{
    public static ShiftSummaryResponse Create(ShiftSummary shift)
    {
        var washingMachines = shift.MachineSummaries
            .Select((x, index) => 
                new ShiftSummaryWashingMachineContract(
                    x.Code, 
                    WashingMachineNameGenerator.GenerateWashingMachineName(index + 1),
                    EfficiencyFormatter.EfficiencyToPercentageString(x.Efficiency),
                    x.AdjustmentTime.ToHhMmSsString(),
                    x.DowntimeTime.ToHhMmSsString(),
                    x.ShutdownTime.ToHhMmSsString()))
            .ToArray();
        
        return new ShiftSummaryResponse(
            shift.Id,
            shift.Name,
            shift.StartDate,
            shift.EndDate,
            EfficiencyFormatter.EfficiencyToPercentageString(shift.Efficiency),
            shift.AdjustmentTime.ToHhMmSsString(),
            shift.DowntimeTime.ToHhMmSsString(),
            shift.ShutdownTime.ToHhMmSsString(),
            washingMachines);
    }
}