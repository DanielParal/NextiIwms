using Nexticz.Module.Mmo.Reporting.Application.LineItems.LineItemQueueBuilder.Models;
using Nexticz.Module.Mmo.Reporting.Domain.Views;

namespace Nexticz.Module.Mmo.Reporting.Application.LineItems.LineItemQueueBuilder;

internal static class CurrentShiftLineItemsBuilder
{
    public static LineItemQueueResponse BuildItems(Guid shiftId, DateTimeOffset kitStartDate, DateTimeOffset kitEndDate, int adjustmentTimeInMinutes, DateTimeOffset lastItemEndDate, LineItemView[] plannedItems)
    {
        if (plannedItems.Length == 0)
        {
            var items = BuildWithoutPlannedItems(shiftId, kitStartDate, kitEndDate, adjustmentTimeInMinutes, lastItemEndDate);
            return new LineItemQueueResponse(items, null);
        }
        
        var orderedLineItems = plannedItems
            .OrderByDescending(x => x.EndDate)
            .ToList();
        
        var lastPlannedItem = orderedLineItems.First();
        LineItemToShorten? itemToShorten = null;
        if (lastPlannedItem.EndDate > kitEndDate)
        {
            itemToShorten = new LineItemToShorten(lastPlannedItem.Id, kitEndDate, lastPlannedItem.EndDate, lastPlannedItem.Type);
            orderedLineItems.Remove(lastPlannedItem);
        }

        if (orderedLineItems.Count == 0)
        {
            var items = BuildWithoutPlannedItems(shiftId, kitStartDate, kitEndDate, adjustmentTimeInMinutes, lastItemEndDate);
            return new LineItemQueueResponse(items, itemToShorten);
        }
        
        var itemsWithPlannedItems = BuildWithPlannedItems(shiftId, kitStartDate, kitEndDate, lastItemEndDate, adjustmentTimeInMinutes, orderedLineItems);
        return new LineItemQueueResponse(itemsWithPlannedItems, itemToShorten);
    }
    
    private static List<LineItemToCreateOrUpdate> BuildWithoutPlannedItems(
        Guid shiftId, DateTimeOffset kitWashingStarted, DateTimeOffset kitWashingEnded, 
        int adjustmentInMinutes, DateTimeOffset lastEndDateOnLine)
    {
        var lineItemsToCreate = new List<LineItemToCreateOrUpdate>();

        if (lastEndDateOnLine == kitWashingStarted)
        {
            lineItemsToCreate.Add(new LineItemToCreateOrUpdate(shiftId, kitWashingStarted, kitWashingEnded, kitWashingEnded - kitWashingStarted, LineItemType.Kit, null));
            return lineItemsToCreate;
        }

        if (lastEndDateOnLine > kitWashingStarted)
        {
            lineItemsToCreate.Add(new LineItemToCreateOrUpdate(shiftId, lastEndDateOnLine, kitWashingEnded, kitWashingEnded - lastEndDateOnLine, LineItemType.Kit, null));
            return lineItemsToCreate;
        }
        
        var inactivityTime = kitWashingStarted - lastEndDateOnLine;

        if (inactivityTime.TotalMinutes <= adjustmentInMinutes)
        {
            lineItemsToCreate.Add(new LineItemToCreateOrUpdate(shiftId, lastEndDateOnLine, kitWashingStarted, kitWashingStarted - lastEndDateOnLine, LineItemType.Adjustment, null));
            lineItemsToCreate.Add(new LineItemToCreateOrUpdate(shiftId, kitWashingStarted, kitWashingEnded, kitWashingEnded - kitWashingStarted, LineItemType.Kit, null));
            return lineItemsToCreate;
        }
        
        var downtimeDuration = inactivityTime - TimeSpan.FromMinutes(adjustmentInMinutes);
        var downtimeEndDate = lastEndDateOnLine + downtimeDuration;
        
        lineItemsToCreate.Add(new LineItemToCreateOrUpdate(shiftId, lastEndDateOnLine, downtimeEndDate, downtimeEndDate - lastEndDateOnLine, LineItemType.Downtime, null));
        lineItemsToCreate.Add(new LineItemToCreateOrUpdate(shiftId, downtimeEndDate, kitWashingStarted, kitWashingStarted - downtimeEndDate, LineItemType.Adjustment, null));
        lineItemsToCreate.Add(new LineItemToCreateOrUpdate(shiftId, kitWashingStarted, kitWashingEnded, kitWashingEnded - kitWashingStarted, LineItemType.Kit, null));
        return lineItemsToCreate;
    }
    
    
    private static List<LineItemToCreateOrUpdate> BuildWithPlannedItems(
        Guid shiftId, DateTimeOffset kitStartDate, DateTimeOffset kitEndDate, 
        DateTimeOffset lastItemEndDateOnLine, int adjustmentTimeInMinutes, 
        List<LineItemView> orderedPlannedItems)
    {
        SyncStartDates(orderedPlannedItems, lastItemEndDateOnLine);
        
        var kitResult = PlannedItemsSplitBuilder.BuildKitParts(shiftId, kitStartDate, kitEndDate, orderedPlannedItems);
        SyncTotalKitTime(kitResult);
        
        orderedPlannedItems.RemoveAll(x => kitResult.PlannedItemsToRemove.Contains(x));
        
        var adjustmentResult = PlannedItemsSplitBuilder.BuildAdjustmentParts(shiftId, lastItemEndDateOnLine, kitResult.LastEndDate, adjustmentTimeInMinutes, orderedPlannedItems);
        orderedPlannedItems.RemoveAll(x => adjustmentResult.PlannedItemsToRemove.Contains(x));
        
        var downTimeResult = PlannedItemsSplitBuilder.BuildDowntimeParts(shiftId, lastItemEndDateOnLine, adjustmentResult.LastEndDate, orderedPlannedItems);
        
        var allLineItems = new List<LineItemToCreateOrUpdate>();
        allLineItems.AddRange(kitResult.NewLineItems);
        allLineItems.AddRange(adjustmentResult.NewLineItems);
        allLineItems.AddRange(downTimeResult.NewLineItems);

        return allLineItems.OrderBy(x => x.StartDate).ToList();
    }
    
    private static void SyncTotalKitTime(PlannedItemsSplitResult kitResult)
    {
        var totalKitTime = kitResult.NewLineItems.Where(x => x.Type == LineItemType.Kit).Sum(x => x.TotalDuration.TotalSeconds);

        // recalculate total duration of split kit
        foreach (var item in kitResult.NewLineItems.Where(x => x.Type == LineItemType.Kit))
        {
            item.UpdateTotalDuration(TimeSpan.FromSeconds(totalKitTime));
        }
    }

    private static void SyncStartDates(List<LineItemView> plannedItems, DateTimeOffset lastItemEndDateOnLine)
    {
        // this should not happen but in case there is planned item with older start date:
        foreach (var plannedItem in plannedItems)
        {
            if (plannedItem.StartDate < lastItemEndDateOnLine)
                plannedItem.StartDate = lastItemEndDateOnLine;
        }
    }
}