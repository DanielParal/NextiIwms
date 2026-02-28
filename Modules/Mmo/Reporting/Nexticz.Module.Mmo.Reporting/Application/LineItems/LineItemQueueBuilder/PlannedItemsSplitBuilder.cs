using Nexticz.Module.Mmo.Reporting.Application.LineItems.LineItemQueueBuilder.Models;
using Nexticz.Module.Mmo.Reporting.Domain.Views;

namespace Nexticz.Module.Mmo.Reporting.Application.LineItems.LineItemQueueBuilder;

internal static class PlannedItemsSplitBuilder
{
    public static PlannedItemsSplitResult BuildDowntimeParts(
        Guid shiftId, DateTimeOffset lastItemEndDateOnLine, DateTimeOffset downtimeEndDate, List<LineItemView> orderedPlannedItems)
    {
        var itemsToCreate = new List<LineItemToCreateOrUpdate>();
        var itemsToRemove = new List<LineItemView>();

        if (lastItemEndDateOnLine >= downtimeEndDate)
            return new PlannedItemsSplitResult([], [], downtimeEndDate);
        
        if (orderedPlannedItems.Count == 0)
        {
            itemsToCreate.Add(new LineItemToCreateOrUpdate(shiftId, lastItemEndDateOnLine, downtimeEndDate, downtimeEndDate-lastItemEndDateOnLine, LineItemType.Downtime, null));
            downtimeEndDate = lastItemEndDateOnLine;
            return new PlannedItemsSplitResult(itemsToCreate, itemsToRemove, downtimeEndDate);
        }
        
        foreach (var item in orderedPlannedItems)
        {
            if (item.StartDate >= lastItemEndDateOnLine)
            {
                if (downtimeEndDate-item.EndDate > TimeSpan.Zero)
                    itemsToCreate.Add(new LineItemToCreateOrUpdate(shiftId, item.EndDate, downtimeEndDate, downtimeEndDate-item.EndDate, LineItemType.Downtime, null));
                
                itemsToCreate.Add(new LineItemToCreateOrUpdate(shiftId, item.StartDate, item.EndDate, item.EndDate - item.StartDate, item.Type, item.Id));
                
                itemsToRemove.Add(item);
                downtimeEndDate = item.StartDate;
                continue;
            }
            
            if (item.StartDate < lastItemEndDateOnLine && item.EndDate > lastItemEndDateOnLine)
            {
                itemsToCreate.Add(new LineItemToCreateOrUpdate(shiftId, item.EndDate, downtimeEndDate, downtimeEndDate-item.EndDate, LineItemType.Downtime, null));
                itemsToCreate.Add(new LineItemToCreateOrUpdate(shiftId, item.StartDate, item.EndDate, item.EndDate - item.StartDate, item.Type, item.Id));
                itemsToRemove.Add(item);
                downtimeEndDate = item.StartDate;
                continue;
            }
            
            if (item.EndDate < lastItemEndDateOnLine)
            {
                itemsToCreate.Add(new LineItemToCreateOrUpdate(shiftId, lastItemEndDateOnLine, downtimeEndDate, downtimeEndDate-lastItemEndDateOnLine, LineItemType.Downtime, null));
                downtimeEndDate = lastItemEndDateOnLine;
                return new PlannedItemsSplitResult(itemsToCreate, itemsToRemove, downtimeEndDate);
            }
        }

        if (downtimeEndDate > lastItemEndDateOnLine)
        {
            itemsToCreate.Add(new LineItemToCreateOrUpdate(shiftId, lastItemEndDateOnLine, downtimeEndDate, downtimeEndDate-lastItemEndDateOnLine, LineItemType.Downtime, null));
            downtimeEndDate = lastItemEndDateOnLine;
        }
        
        return new PlannedItemsSplitResult(itemsToCreate, itemsToRemove, downtimeEndDate);
    }
    
    internal static PlannedItemsSplitResult BuildAdjustmentParts(
        Guid shiftId, DateTimeOffset lastItemEndDateOnLine, DateTimeOffset adjustmentEndDate, int adjustmentTimeInMinutes, List<LineItemView> orderedPlannedItems)
    {
        var itemsToCreate = new List<LineItemToCreateOrUpdate>();
        var itemsToRemove = new List<LineItemView>();

        var adjustmentStartDate = GetAdjustmentStartDate(lastItemEndDateOnLine, adjustmentEndDate, adjustmentTimeInMinutes);

        if (lastItemEndDateOnLine >= adjustmentEndDate)
            return new PlannedItemsSplitResult([], [], adjustmentEndDate);
        
        if (orderedPlannedItems.Count == 0)
        {
            itemsToCreate.Add(new LineItemToCreateOrUpdate(shiftId, adjustmentStartDate, adjustmentEndDate, adjustmentEndDate-adjustmentStartDate, LineItemType.Adjustment, null));
            adjustmentEndDate = adjustmentStartDate;
            return new PlannedItemsSplitResult(itemsToCreate, itemsToRemove, adjustmentEndDate);
        }
        
        foreach (var item in orderedPlannedItems)
        {
            if (item.StartDate >= adjustmentStartDate)
            {
                if (adjustmentEndDate-item.EndDate > TimeSpan.Zero)
                    itemsToCreate.Add(new LineItemToCreateOrUpdate(shiftId, item.EndDate, adjustmentEndDate, adjustmentEndDate-item.EndDate, LineItemType.Adjustment, null));
                
                itemsToCreate.Add(new LineItemToCreateOrUpdate(shiftId, item.StartDate, item.EndDate, item.EndDate - item.StartDate, item.Type, item.Id));
                adjustmentStartDate = RecalculateAdjustmentStartDate(adjustmentStartDate, lastItemEndDateOnLine, item.EndDate - item.StartDate);
                
                itemsToRemove.Add(item);
                adjustmentEndDate = item.StartDate;
                continue;
            }
            
            if (item.StartDate < adjustmentStartDate && item.EndDate > adjustmentStartDate)
            {
                itemsToCreate.Add(new LineItemToCreateOrUpdate(shiftId, item.EndDate, adjustmentEndDate, adjustmentEndDate-item.EndDate, LineItemType.Adjustment, null));
                itemsToCreate.Add(new LineItemToCreateOrUpdate(shiftId, item.StartDate, item.EndDate, item.EndDate - item.StartDate, item.Type, item.Id));
                
                adjustmentStartDate = RecalculateAdjustmentStartDate(adjustmentStartDate, lastItemEndDateOnLine, item.EndDate - item.StartDate);
                itemsToRemove.Add(item);
                adjustmentEndDate = item.StartDate;
                continue;
            }
            
            if (item.EndDate < adjustmentStartDate)
            {
                itemsToCreate.Add(new LineItemToCreateOrUpdate(shiftId, adjustmentStartDate, adjustmentEndDate, adjustmentEndDate-adjustmentStartDate, LineItemType.Adjustment, null));
                adjustmentEndDate = adjustmentStartDate;
                return new PlannedItemsSplitResult(itemsToCreate, itemsToRemove, adjustmentEndDate);
            }
        }

        if (adjustmentEndDate > adjustmentStartDate)
        {
            itemsToCreate.Add(new LineItemToCreateOrUpdate(shiftId, adjustmentStartDate, adjustmentEndDate, adjustmentEndDate-adjustmentStartDate, LineItemType.Adjustment, null));
            adjustmentEndDate = adjustmentStartDate;
        }
        
        return new PlannedItemsSplitResult(itemsToCreate, itemsToRemove, adjustmentEndDate);
    }
    
    internal static PlannedItemsSplitResult BuildKitParts(
        Guid shiftId, DateTimeOffset kitStartDate, DateTimeOffset kitEndDate, List<LineItemView> orderedPlannedItems)
    {
        var itemsToCreate = new List<LineItemToCreateOrUpdate>();
        var itemsToRemove = new List<LineItemView>();
        var lastEndDate = kitEndDate;
        foreach (var item in orderedPlannedItems)
        {
            if (item.StartDate >= kitStartDate)
            {
                if (kitEndDate-item.EndDate > TimeSpan.Zero)
                    itemsToCreate.Add(new LineItemToCreateOrUpdate(shiftId, item.EndDate, lastEndDate, lastEndDate-item.EndDate, LineItemType.Kit, null));
                
                itemsToCreate.Add(new LineItemToCreateOrUpdate(shiftId, item.StartDate, item.EndDate, item.EndDate - item.StartDate, item.Type, item.Id));
                itemsToRemove.Add(item);
                lastEndDate = item.StartDate;
                
                if (item.StartDate == kitStartDate)
                    return new PlannedItemsSplitResult(itemsToCreate, itemsToRemove, lastEndDate);
                
                continue;
            }

            if (item.StartDate < kitStartDate && item.EndDate > kitStartDate)
            {
                itemsToCreate.Add(new LineItemToCreateOrUpdate(shiftId, item.EndDate, lastEndDate, lastEndDate-item.EndDate, LineItemType.Kit, null));
                itemsToCreate.Add(new LineItemToCreateOrUpdate(shiftId, item.StartDate, item.EndDate, item.EndDate - item.StartDate, item.Type, item.Id));
                itemsToRemove.Add(item);
                lastEndDate = item.StartDate;
                return new PlannedItemsSplitResult(itemsToCreate, itemsToRemove, lastEndDate);
            }
            
            if (item.EndDate < kitStartDate)
            {
                itemsToCreate.Add(new LineItemToCreateOrUpdate(shiftId, kitStartDate, lastEndDate, lastEndDate-kitStartDate, LineItemType.Kit, null));
                lastEndDate = kitStartDate;
                return new PlannedItemsSplitResult(itemsToCreate, itemsToRemove, lastEndDate);
            }
        }

        if (lastEndDate > kitStartDate)
        {
            itemsToCreate.Add(new LineItemToCreateOrUpdate(shiftId, kitStartDate, lastEndDate, lastEndDate-kitStartDate, LineItemType.Kit, null));
            lastEndDate = kitStartDate;
        }
        
        return new PlannedItemsSplitResult(itemsToCreate, itemsToRemove, lastEndDate);
    }

    private static DateTimeOffset GetAdjustmentStartDate(DateTimeOffset lastItemEndDateOnLine, DateTimeOffset lastEndDate, int adjustmentTimeInMinutes)
    {
        var calculatedStartDate = lastEndDate.AddMinutes(-adjustmentTimeInMinutes);
        return calculatedStartDate > lastItemEndDateOnLine ? calculatedStartDate : lastItemEndDateOnLine;
    }

    private static DateTimeOffset RecalculateAdjustmentStartDate(DateTimeOffset currentAdjustmentStartDate, DateTimeOffset lastItemEndDateOnLine,
        TimeSpan breakDuration)
    {
        var adjustmentStartDate = currentAdjustmentStartDate.AddMinutes(-breakDuration.TotalMinutes);
        return adjustmentStartDate > lastItemEndDateOnLine ? adjustmentStartDate : lastItemEndDateOnLine;
    }
    
}