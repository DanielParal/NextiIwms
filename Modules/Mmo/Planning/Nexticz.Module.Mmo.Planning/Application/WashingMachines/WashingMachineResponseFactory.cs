using Nexticz.Module.Mmo.Planning.Contracts.WashingMachines;
using Nexticz.Module.Mmo.Washing.Contracts.WashingMachineSoses;
using Nexticz.Module.Mmo.Planning.Application.WashingMachineTimeTables.Models;
using Nexticz.Module.Mmo.Planning.Domain.BatchEntity;

namespace Nexticz.Module.Mmo.Planning.Application.WashingMachines;

internal class WashingMachineResponseFactory
{
    public static WashingMachineResponse Create(WashingMachineTimeTable timeTable, WashingMachineSosResponse[] sosResponses)
    {
        var queues = timeTable.Queues
            .Select(q => new WashingMachineLineQueueContract(
                q.Code,
                q.Items.Select(GetItemContract)
                    .ToArray(),
                q.BatchCount,
                q.IsActive,
                q.Items.Any(IsItemInWashing),
                IsAnyBatchInWashingInSisterLine(timeTable.Queues.Count < 2, timeTable, q.Code),
                q.GetLastEndDate()
            ))
            .ToArray();
            
        return new WashingMachineResponse(
            timeTable.Code,
            sosResponses.FirstOrDefault(x => x.Code.Equals(timeTable.Code, StringComparison.InvariantCultureIgnoreCase))?.IsHelpNeeded ?? false,
            (WashingMachineStatusContract)timeTable.Status,
            queues
        );
    }

    private static bool IsAnyBatchInWashingInSisterLine(bool isOneLine, WashingMachineTimeTable washingMachineTimeTable, string currentLineQueueCode)
    {
        if (isOneLine)
            return false;

        var secondQueue = washingMachineTimeTable.Queues.First(x => !x.Code.Equals(currentLineQueueCode, StringComparison.InvariantCultureIgnoreCase));
        
        return secondQueue.Items.Any(IsItemInWashing);
    }

    private static bool IsItemInWashing(TimeTableItem items)
    {
        return items is BatchItem { Status: BatchStatus.Washing };
    }
    
    private static WashingMachineLineQueueItemContract GetItemContract(TimeTableItem item)
    {
        return item switch
        {
            BatchItem batchItem => new WashingMachineLineQueueItemContract(
                WashingMachineLineQueueItemTypeContract.Batch,
                batchItem.TimeTableSchedule.Start,
                batchItem.TimeTableSchedule.End,
                batchItem.Id,
                batchItem.KitCode,
                batchItem.PackagingCode,
                batchItem.SisterPackagingCode,
                batchItem.SisterBatchId is not null,
                batchItem.KitsCount,
                batchItem.KitFinished,
                batchItem.Index,
                batchItem.SisterBatchIndex,
                (BatchStatusContract)batchItem.Status
            ),
            AdjustmentItem adjustmentItem => new WashingMachineLineQueueItemContract(
                WashingMachineLineQueueItemTypeContract.WashingMachineAdjustment,
                adjustmentItem.TimeTableSchedule.Start,
                adjustmentItem.TimeTableSchedule.End,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null
                ),
            DowntimeItem downtimeItem => new WashingMachineLineQueueItemContract(
                WashingMachineLineQueueItemTypeContract.Downtime,
                downtimeItem.TimeTableSchedule.Start,
                downtimeItem.TimeTableSchedule.End,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null
            ),
            _ => throw new NotSupportedException($"Unsupported TimeTableItem type: {item.GetType().Name}")
        };
    }
}