using ErrorOr;
using Nexticz.Module.Mmo.Reporting.Application.LineItems.LineItemQueueBuilder.Models;
using Nexticz.Module.Mmo.Reporting.Domain.ShiftAggregate;
using Nexticz.Module.Mmo.Reporting.Domain.Views;

namespace Nexticz.Module.Mmo.Reporting.Application.LineItems.LineItemQueueBuilder;

internal class NextToLastShiftLineItemsBuilder
{
    internal static async Task<ShouldHandleNextToLastShiftResult> ShouldHandleNextToLastShiftItemsAsync(
        Shift currentShift,
        LastItemPerLineView? lastItemOnLine, 
        Func<Task<ErrorOr<Shift>>> getNextToLastShiftAction)
    {
        if (lastItemOnLine is null || lastItemOnLine.ShiftId == currentShift.Id)
            return new ShouldHandleNextToLastShiftResult(false, null);
        
        var nextToLastShift = await getNextToLastShiftAction();
        
        if (nextToLastShift.IsError)
            return new ShouldHandleNextToLastShiftResult(false, null);
        
        if (nextToLastShift.Value.Schedule.End <= lastItemOnLine.LastFinishedDate)
            return new ShouldHandleNextToLastShiftResult(false, null);
        
        return new ShouldHandleNextToLastShiftResult(true, nextToLastShift.Value);
    }

    internal static NextToLastShiftBuilderResult Build(
        DateTimeOffset lastItemEndDateOnLine, DateTimeOffset optimalNextKitStartDate, Shift nextToLastShift, List<LineItemView> plannedItems)
    {
        if (lastItemEndDateOnLine < nextToLastShift.Schedule.Start)
            lastItemEndDateOnLine = nextToLastShift.Schedule.Start;
        
        if (lastItemEndDateOnLine >= optimalNextKitStartDate)
            return new NextToLastShiftBuilderResult([], [], lastItemEndDateOnLine);
        
        var nextToLastShiftItemsEndDate = new[] {optimalNextKitStartDate, nextToLastShift.Schedule.End}.Min();
        
        var lastItemInNextToLastShiftEndDate = lastItemEndDateOnLine > nextToLastShiftItemsEndDate ? lastItemEndDateOnLine : nextToLastShiftItemsEndDate;
        var selectedPlannedItems = plannedItems.Where(x => x.EndDate <= lastItemInNextToLastShiftEndDate).ToList();
        var downTimeResult = PlannedItemsSplitBuilder.BuildDowntimeParts(nextToLastShift.Id, lastItemEndDateOnLine, lastItemInNextToLastShiftEndDate, selectedPlannedItems);
        return new NextToLastShiftBuilderResult(downTimeResult.NewLineItems, downTimeResult.PlannedItemsToRemove, lastItemInNextToLastShiftEndDate);
    }
}