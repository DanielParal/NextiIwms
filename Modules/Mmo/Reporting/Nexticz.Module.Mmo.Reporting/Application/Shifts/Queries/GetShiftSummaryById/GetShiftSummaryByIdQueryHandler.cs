using MediatR;
using Nexticz.Module.Mmo.Settings.Contracts.WashingMachines.Queries;
using Nexticz.Lib.Shared.Time;
using Nexticz.Module.Mmo.Reporting.Application.LineItems.Queries.GetLineItemsByShiftId;
using Nexticz.Module.Mmo.Reporting.Application.Shifts.Models;
using Nexticz.Module.Mmo.Reporting.Domain.ShiftAggregate;
using Nexticz.Module.Mmo.Reporting.Domain.Views;

namespace Nexticz.Module.Mmo.Reporting.Application.Shifts.Queries.GetShiftSummaryById;

internal class GetShiftSummaryByIdQueryHandler(
    ISender sender,
    IClock clock) 
    : IRequestHandler<GetShiftSummaryByIdQuery, ShiftSummary>
{
    public async Task<ShiftSummary> Handle(GetShiftSummaryByIdQuery request, CancellationToken cancellationToken)
    {
        var totalLineItems = await sender.Send(new GetLineItemsByShiftIdQuery(request.Shift.Id), cancellationToken);
        
        var automaticLineItems = totalLineItems
            .Where(x => !x.IsPlanned)
            .ToList();

        var washingMachinesFromSettings = await sender.Send(new GetWashingMachineResponsesQuery(), cancellationToken);
        var shiftWashingMachinesSummaries = new List<ShiftWashingMachineSummary>();
        foreach (var washingMachineFromSetting in washingMachinesFromSettings)
        {
            var shiftWashingLinesSummaries = new List<ShiftWashingLineSummary>();
            foreach (var line in washingMachineFromSetting.WashingMachineLines)
            {
                var lineItems = automaticLineItems.Where(l => l.LineCode == line.Code).ToArray();
                var washingLineSummary = GetWashingLineSummary(line.Code, request.Shift, lineItems);
                shiftWashingLinesSummaries.Add(washingLineSummary);
            }
            
            var washingMachineSummary = new ShiftWashingMachineSummary
            {
                Code = washingMachineFromSetting.Code,
                LineSummaries = shiftWashingLinesSummaries
                    .ToList()
            };
            shiftWashingMachinesSummaries.Add(washingMachineSummary);
        }

        var shiftSummary = new ShiftSummary
        {
            Id = request.Shift.Id,
            Name = request.Shift.Name,
            StartDate = request.Shift.Schedule.Start,
            EndDate = request.Shift.Schedule.End,
            MachineSummaries = shiftWashingMachinesSummaries
        };
        
        return shiftSummary;
    }

    private ShiftWashingLineSummary GetWashingLineSummary(string lineCode, Shift shift, LineItemView[] lineItems)
    {
        var kits = lineItems.Where(x => x.Type == LineItemType.Kit).ToList();
        var adjustments = lineItems.Where(x => x.Type == LineItemType.Adjustment).ToList();
        var impactfulDowntime = lineItems.Where(x => x is { Type: LineItemType.Downtime, AffectProductivity: true }).ToList();
        var nonImpactfulDowntime = lineItems.Where(x => x is { Type: LineItemType.Downtime, AffectProductivity: false }).ToList();
        var impactfulShutdownTime = lineItems.Where(x => x is { Type: LineItemType.Shutdown, AffectProductivity: true }).ToList();
        var nonImpactfulShutdownTime = lineItems.Where(x => x is { Type: LineItemType.Shutdown, AffectProductivity: false }).ToList();
        var shiftEndDate = shift.Schedule.End <= clock.UtcNowOffset ? shift.Schedule.End : clock.UtcNowOffset;
            
        var lineSummary = new ShiftWashingLineSummary
        {
            Code = lineCode,
            AdjustmentTime = TimeSpan.FromTicks(adjustments.Sum(x => (x.EndDate - x.StartDate).Ticks)),
            NonImpactProductivityDowntimeTime = TimeSpan.FromTicks(nonImpactfulDowntime.Sum(x => (x.EndDate - x.StartDate).Ticks)),
            ImpactProductivityDowntimeTime = TimeSpan.FromTicks(impactfulDowntime.Sum(x => (x.EndDate - x.StartDate).Ticks)),
            NonImpactProductivityShutdownTime = TimeSpan.FromTicks(nonImpactfulShutdownTime.Sum(x => (x.EndDate - x.StartDate).Ticks)),
            ImpactProductivityShutdownTime = TimeSpan.FromTicks(impactfulShutdownTime.Sum(x => (x.EndDate - x.StartDate).Ticks)),
            LineTotalTime = shiftEndDate - shift.Schedule.Start,
            RealWashingTime = TimeSpan.FromTicks(kits.Sum(x => (x.EndDate - x.StartDate).Ticks)),
            OptimalWashingTime = TimeSpan.FromTicks(kits.Sum(x => x.OptimalKitDuration!.Value.Ticks)),
        };

        return lineSummary;
    }
}