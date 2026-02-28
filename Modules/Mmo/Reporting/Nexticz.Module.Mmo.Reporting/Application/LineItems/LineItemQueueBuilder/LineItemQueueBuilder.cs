using ErrorOr;
using Nexticz.Module.Mmo.Reporting.Application.LineItems.LineItemQueueBuilder.Models;
using Nexticz.Module.Mmo.Reporting.Domain.ShiftAggregate;
using Nexticz.Module.Mmo.Reporting.Domain.Views;

namespace Nexticz.Module.Mmo.Reporting.Application.LineItems.LineItemQueueBuilder;

internal static class LineItemQueueBuilder
{
    public static async Task<LineItemQueueResponse> BuildAsync(
        Shift currentShift,
        DateTimeOffset kitStartDate, 
        DateTimeOffset kitEndDate,
        TimeSpan optimalKitDuration,
        LastItemPerLineView? lastItemOnLine, 
        int adjustmentTimeInMinutes,
        LineItemView[] plannedItems, 
        Func<Task<ErrorOr<Shift>>> getNextToLastShiftAction)
    {
        var shouldHandleNextToLastShift = 
            await NextToLastShiftLineItemsBuilder.ShouldHandleNextToLastShiftItemsAsync(
                currentShift, lastItemOnLine, getNextToLastShiftAction);

        // we should handle next to last shift items only for the first kit on the line in shift
        if (shouldHandleNextToLastShift.ShouldHandle)
        {
            return BuildItemsForNextToLastShiftAndForCurrentShift(
                currentShift, shouldHandleNextToLastShift.NextToLastShift!, kitEndDate, 
                optimalKitDuration, lastItemOnLine!, adjustmentTimeInMinutes, plannedItems);
        }
        
        var lastItemEndDate = lastItemOnLine?.LastFinishedDate ?? currentShift.Schedule.Start;
        return CurrentShiftLineItemsBuilder.BuildItems(currentShift.Id, kitStartDate, kitEndDate, adjustmentTimeInMinutes, lastItemEndDate, plannedItems);
    }

    private static LineItemQueueResponse BuildItemsForNextToLastShiftAndForCurrentShift(
        Shift currentShift,
        Shift nextToLastShift,
        DateTimeOffset kitEndDate,
        TimeSpan optimalKitDuration,
        LastItemPerLineView lastItemOnLine, 
        int adjustmentTimeInMinutes,
        LineItemView[] plannedItems)
    {
        var optimalKitStartDate = kitEndDate - optimalKitDuration;
        var nextToLastShiftResult = NextToLastShiftLineItemsBuilder.Build(
            lastItemOnLine.LastFinishedDate,
            optimalKitStartDate,
            nextToLastShift, 
            plannedItems.ToList());
            
        var plannedItemsExceptFromNextToLastShift = 
            plannedItems.Except(nextToLastShiftResult.PlannedItemsToRemove).ToArray();
        
        var dateWhenShouldCurrentLineStartItems = 
            GetDateWhenShouldCurrentLineStartItems(
                nextToLastShiftResult.LastItemOnLineEndDate, optimalKitStartDate,
                nextToLastShift.Schedule.End, currentShift.Schedule.Start);
            
        var lineItemQueueResponse = CurrentShiftLineItemsBuilder.BuildItems(
            currentShift.Id, optimalKitStartDate, kitEndDate, adjustmentTimeInMinutes, 
            dateWhenShouldCurrentLineStartItems, plannedItemsExceptFromNextToLastShift);
        lineItemQueueResponse.AddLineItemsToCreateOrUpdate(nextToLastShiftResult.LineItemsToCreateOrUpdate);
                
        return lineItemQueueResponse;
    }
    
    private static DateTimeOffset GetDateWhenShouldCurrentLineStartItems(
        DateTimeOffset lastItemOnLineEndDate, DateTimeOffset optimalKitStartDate, 
        DateTimeOffset nextToLastShiftEndDate, DateTimeOffset currentShiftStartDate)
    {
        // shifts follow each other, and there is no gap between shifts
        if (nextToLastShiftEndDate == currentShiftStartDate)
            return lastItemOnLineEndDate;

        return new[] { optimalKitStartDate, currentShiftStartDate }.Min();
    } 
}