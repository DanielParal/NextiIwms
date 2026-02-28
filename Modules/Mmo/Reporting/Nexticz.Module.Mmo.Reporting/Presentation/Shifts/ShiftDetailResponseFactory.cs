using MediatR;
using Nexticz.Module.Mmo.Reporting.Contracts.Shifts;
using Nexticz.Module.Mmo.Washing.Contracts.WashingMachineSoses.Queries;
using Nexticz.Module.Mmo.Reporting.Application.LineItems.Queries;
using Nexticz.Module.Mmo.Reporting.Application.LineItems.Queries.GetLineItemsByShiftId;
using Nexticz.Module.Mmo.Reporting.Application.Shifts.Queries.GetShiftStatus;
using Nexticz.Module.Mmo.Reporting.Application.Shifts.Queries.GetUnapprovedShiftStatusViews;
using Nexticz.Module.Mmo.Reporting.Application.WashingMachineSpeeds.Queries.GetWashingMachineSpeedsByTimeRange;
using Nexticz.Module.Mmo.Reporting.Domain.ShiftAggregate;

namespace Nexticz.Module.Mmo.Reporting.Presentation.Shifts;

internal static class ShiftDetailResponseFactory
{
    public static async Task<ShiftDetailResponse> CreateAsync(
        Shift shift, ISender sender, CancellationToken cancellationToken)
    {
        var lineItems = await sender.Send(new GetLineItemsByShiftIdQuery(shift.Id), cancellationToken);
        
        var startDateToSelect = shift.Schedule.Start.AddHours(-1);
        var endDateToSelect = shift.Schedule.End.AddHours(1);
        var speeds = await sender.Send(new GetWashingMachineSpeedsByTimeRangeQuery(startDateToSelect, endDateToSelect), cancellationToken);
        var sosResponses = await sender.Send(new GetWashingMachineSosResponsesQuery(), cancellationToken);
        
        var washingMachines = 
            shift.WashingMachines
                .Select((x, index) => 
                    new WashingMachineDetailContract(
                        x.Code, 
                        WashingMachineNameGenerator.GenerateWashingMachineName(index + 1),
                        sosResponses.FirstOrDefault(y => y.Code.Equals(x.Code, StringComparison.InvariantCultureIgnoreCase))?.IsHelpNeeded ?? false))
                .ToArray();
        
        const string speedLineCode = "Speed";
        var washingMachineLines = 
            shift.WashingMachines.SelectMany(machine => 
                machine.Lines.Select((line, index) => 
                    new WashingMachineLineDetailContract(line.Code[(machine.Code.Length + 1)..], WashingMachineNameGenerator.GenerateWashingMachineLineName(index + 1))))
                .Concat([new WashingMachineLineDetailContract(speedLineCode, WashingMachineNameGenerator.GenerateWashingMachineLineSpeedName())])
                .DistinctBy(x => x.Code)
            .ToArray();
        
        var lineItemContracts = lineItems.Select(LineItemContractFactory.Create).ToList();
        lineItemContracts.AddRange(speeds.Select(x => LineItemContractFactory.Create(x, speedLineCode)));
        var lineItemsForReview = 
            LineItemForReviewFilter.FilterLineItemsForReview(lineItems)
            .Select(LineItemContractFactory.Create).ToArray();
        
        var status = (ShiftStatusContract)await GetShiftStatusAsync(shift, sender, cancellationToken);
        
        return new ShiftDetailResponse(
            washingMachines, 
            washingMachineLines, 
            lineItemContracts.ToArray(),
            lineItemsForReview,
            shift.Schedule.Start.Hour,
            shift.Schedule.End.Hour,
            (int)Math.Ceiling((shift.Schedule.End - shift.Schedule.Start).TotalHours),
            status);
    }

    private static async Task<ShiftStatus> GetShiftStatusAsync(Shift shift, ISender sender, CancellationToken cancellationToken)
    {
        var unapprovedShifts = await sender.Send(new GetUnapprovedShiftStatusViewsQuery(), cancellationToken);
        return await sender.Send(new GetShiftStatusQuery(shift, unapprovedShifts), cancellationToken);
    }
}