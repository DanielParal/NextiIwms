using MediatR;
using Nexticz.Module.Mmo.Reporting.Contracts.Shifts;
using Nexticz.Module.Mmo.Reporting.Contracts.Shifts.Queries;
using Nexticz.Module.Mmo.Settings.Contracts.WashingMachines;
using Nexticz.Module.Mmo.Settings.Contracts.WashingMachines.Queries;
using Nexticz.Module.Mmo.SharedKernel.Efficiencies;
using Nexticz.Module.Mmo.Washing.Contracts.WashingStates;
using Nexticz.Module.Mmo.Washing.Application.Batches.Queries.GetBatches;
using Nexticz.Module.Mmo.Washing.Application.WashingMachineSoses.Queries.GetWashingMachineSoses;
using Nexticz.Module.Mmo.Washing.Application.WashingMachineSpeeds.Queries.GetWashingMachineSpeedByCode;
using Nexticz.Module.Mmo.Washing.Domain.BatchAggregate;
using Nexticz.Module.Mmo.Washing.Domain.WashingMachineSosAggregate;

namespace Nexticz.Module.Mmo.Washing.Application.WashingStates.Queries.GetWashingStateResponse;

internal class GetWashingStateResponseQueryHandler(ISender sender) 
    : IRequestHandler<GetWashingStateResponseQuery, WashingStateResponse>
{
    public async Task<WashingStateResponse> Handle(GetWashingStateResponseQuery request, CancellationToken cancellationToken)
    {
        var washingMachinesFromSettings = await sender.Send(new GetWashingMachineResponsesQuery(), cancellationToken);
        var batches = await sender.Send(new GetBatchesQuery(), cancellationToken);
        var shiftSummaries = await sender.Send(new GetWashingStateShiftsQuery(), cancellationToken);
        var shifts = GetWashingStateShifts(shiftSummaries);
        var washingMachineSoses = await sender.Send(new GetWashingMachineSosesQuery(), cancellationToken);
        return new WashingStateResponse(
            shifts.LastShift,
            shifts.NextToLastShift,
            await GetWashingStateMachines(washingMachinesFromSettings, batches, shiftSummaries.LastShift?.MachineSummaries ?? [], washingMachineSoses, cancellationToken));
    }

    private (WashingStateShiftContract LastShift, WashingStateShiftContract NextToLastShift) GetWashingStateShifts(WashingStateShiftsContract shiftsFromReporting)
    {
        var lastShift = shiftsFromReporting.LastShift is null ?
            new WashingStateShiftContract(0, 0, 0, EfficiencyFormatter.ZeroToPercentageString) :
            new WashingStateShiftContract(
                (int)shiftsFromReporting.LastShift.AdjustmentTime.TotalMinutes, (int)shiftsFromReporting.LastShift.ShutdownTime.TotalMinutes,
                (int)shiftsFromReporting.LastShift.DowntimeTime.TotalMinutes, shiftsFromReporting.LastShift.Efficiency);
        
        var nextToLastShift = shiftsFromReporting.NextToLastShift is null ?
            new WashingStateShiftContract(0, 0, 0, EfficiencyFormatter.ZeroToPercentageString) :
            new WashingStateShiftContract(
                (int)shiftsFromReporting.NextToLastShift.AdjustmentTime.TotalMinutes, (int)shiftsFromReporting.NextToLastShift.ShutdownTime.TotalMinutes,
                (int)shiftsFromReporting.NextToLastShift.DowntimeTime.TotalMinutes, shiftsFromReporting.NextToLastShift.Efficiency);
        
        return (lastShift, nextToLastShift);
    }

    private async Task<WashingStateMachineContract[]> GetWashingStateMachines(
        WashingMachineResponse[] washingMachineResponses, 
        Batch[] batches, 
        WashingStateMachineSummaryContract[] washingStateSummariesFromReporting,
        WashingMachineSos[] washingMachineSoses,
        CancellationToken cancellationToken)
    {
        var result = new List<WashingStateMachineContract>();
        foreach (var washingMachineResponse in washingMachineResponses)
        {
            var lines = await GetWashingStateLines(washingMachineResponse, batches, cancellationToken);
            var washingStateSummary = washingStateSummariesFromReporting.FirstOrDefault(x => x.Code == washingMachineResponse.Code);
            var washingMachineSos = washingMachineSoses.FirstOrDefault(x => x.Code == washingMachineResponse.Code);
            result.Add(
                new WashingStateMachineContract(
                    washingMachineResponse.Code,
                    washingStateSummary?.AdjustmentTime.TotalMinutes is null ? 0 : (int)washingStateSummary.AdjustmentTime.TotalMinutes!,
                    washingStateSummary?.ShutdownTime is null ? 0 : (int)washingStateSummary.ShutdownTime.TotalMinutes!,
                    washingStateSummary?.DowntimeTime is null ? 0 : (int)washingStateSummary.DowntimeTime.TotalMinutes!,
                    washingStateSummary?.Efficiency ?? EfficiencyFormatter.ZeroToPercentageString,
                    washingMachineSos?.IsHelpNeeded ?? false,
                    lines));
        }
        
        return result.ToArray();
    }

    private async Task<WashingStateLineContract[]> GetWashingStateLines(WashingMachineResponse washingMachine, Batch[] batches, CancellationToken cancellationToken)
    {
        var washingMachineSpeed = (await sender.Send(new GetWashingMachineSpeedByCodeQuery(washingMachine.Code), cancellationToken))?.Speed ?? 0;
        
        return washingMachine.WashingMachineLines
            .Select(lineContract =>
            {
                var batch = batches.FirstOrDefault(x => x.LineCode == lineContract.Code);
                var batchInState = batch is null ? null : GetWashingStateBatch(batch);

                return new WashingStateLineContract(
                    lineContract.Code,
                    washingMachineSpeed,
                    batchInState);
            })
            .ToArray();
    }
    
    private static WashingStateBatchContract GetWashingStateBatch(Batch batch)
    {
        var efficiency = batch.KitWashCycles.Count == 0 ? 0 : batch.KitWashCycles.Average(x => x.Efficiency);
        var formattedEfficiency = EfficiencyFormatter.EfficiencyToPercentageString(efficiency);
        var remainingTime = (batch.PlannedKitsCount - batch.KitWashCycles.Count) * batch.OptimalKitDuration;
        var remainingTimeInMinutes = (int)Math.Ceiling(remainingTime.TotalMinutes);
        return new WashingStateBatchContract(
            batch.Id,
            batch.SisterBatchId,
            batch.KitCode,
            batch.PackagingCode,
            batch.KitWashCycles.Count,
            batch.PlannedKitsCount,
            formattedEfficiency,
            remainingTimeInMinutes);
    }
}