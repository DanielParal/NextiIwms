using MediatR;
using Nexticz.Module.Mmo.Reporting.Contracts.Shifts;
using Nexticz.Module.Mmo.Reporting.Contracts.Shifts.Queries;
using Nexticz.Module.Mmo.Reporting.Application.Shifts.Queries.GetLastShift;
using Nexticz.Module.Mmo.Reporting.Application.Shifts.Queries.GetNextToLastShift;
using Nexticz.Module.Mmo.Reporting.Application.Shifts.Queries.GetShiftSummaryById;
using Nexticz.Module.Mmo.Reporting.Domain.ShiftAggregate;
using Nexticz.Module.Mmo.SharedKernel.Efficiencies;

namespace Nexticz.Module.Mmo.Reporting.Application.Shifts.Queries.GetWashingStateShifts;

internal class GetWashingStateShiftsQueryHandler(
    ISender sender) 
    : IRequestHandler<GetWashingStateShiftsQuery, WashingStateShiftsContract>
{
    public async Task<WashingStateShiftsContract> Handle(GetWashingStateShiftsQuery request, CancellationToken cancellationToken)
    {
        var lastShift = await sender.Send(new GetLastShiftQuery(), cancellationToken);
        var washingStateLastShift = lastShift.IsError ? null : await GetWashingStateShiftsContractAsync(lastShift.Value, cancellationToken);
        
        var nextToLastShift = await sender.Send(new GetNextToLastShiftQuery(), cancellationToken);
        var washingStateNextToLastShift = nextToLastShift.IsError ? null : await GetWashingStateShiftsContractAsync(nextToLastShift.Value, cancellationToken);
        
        return new WashingStateShiftsContract(washingStateLastShift, washingStateNextToLastShift);
    }

    private async Task<WashingStateShiftSummaryContract> GetWashingStateShiftsContractAsync(
        Shift shift,
        CancellationToken cancellationToken)
    {
        var shiftSummary = await sender.Send(new GetShiftSummaryByIdQuery(shift), cancellationToken);
        
        return new WashingStateShiftSummaryContract(
            shiftSummary.Id,
            shiftSummary.AdjustmentTime,
            shiftSummary.ShutdownTime,
            shiftSummary.DowntimeTime,
            EfficiencyFormatter.EfficiencyToPercentageString(shiftSummary.Efficiency),
            shiftSummary.MachineSummaries
                .Select(m => new WashingStateMachineSummaryContract(
                    m.Code,
                    m.AdjustmentTime,
                    m.ShutdownTime,
                    m.DowntimeTime,
                    EfficiencyFormatter.EfficiencyToPercentageString(m.Efficiency)))
                .ToArray());
    }
}