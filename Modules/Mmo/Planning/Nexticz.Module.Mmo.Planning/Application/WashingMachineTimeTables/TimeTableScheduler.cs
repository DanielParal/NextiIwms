using Nexticz.Lib.Shared.Time;
using Nexticz.Module.Mmo.Planning.Application.WashingMachineTimeTables.Models;
using Nexticz.Module.Mmo.Planning.Domain.BatchEntity;
using Nexticz.Module.Mmo.Planning.Domain.LineQueueEntity;

namespace Nexticz.Module.Mmo.Planning.Application.WashingMachineTimeTables;

internal class TimeTableScheduler
{
    private readonly TimeSpan _adjustmentDuration;
    private readonly DateTimeOffset _firstStartDate;
    public TimeTableScheduler(TimeSpan adjustmentDuration, IClock clock)
    {
        _adjustmentDuration = adjustmentDuration;
        _firstStartDate = clock.UtcNowOffset;
    }
    
    public List<TimeTableQueue> Schedule(IReadOnlyList<LineQueue> lineQueues)
    {
        if (lineQueues.Count == 0) 
            return [];
        
        if (lineQueues.Count == 1) 
            return [ScheduleSingleQueue(lineQueues[0].WashingMachineLineCode, lineQueues[0].IsActive, lineQueues[0].Batches.ToList())];
        
        return ScheduleTwoLines(lineQueues);
    }

    private TimeTableQueue ScheduleSingleQueue(string code, bool isActive, List<Batch> batches)
    {
        return new TimeTableQueue { Code = code, IsActive = isActive, Items = GenerateSingleLineItems(batches) };
    }

    private List<TimeTableQueue> ScheduleTwoLines(IReadOnlyList<LineQueue> lineQueues)
    {
        var queues = new List<TimeTableQueue>
        {
            new() { Code = lineQueues[0].WashingMachineLineCode, IsActive = lineQueues[0].IsActive }, 
            new() { Code = lineQueues[1].WashingMachineLineCode, IsActive = lineQueues[1].IsActive }
        };
        var queue1 = queues[0];
        var queue2 = queues[1];
        var line1Batches = new List<Batch>(lineQueues[0].Batches);
        var line2Batches = new List<Batch>(lineQueues[1].Batches);

        while (line1Batches.Count != 0)
        {
            var batchesUntilSister1 = GetBatchesUntilSister(line1Batches);

            if (batchesUntilSister1.Count != 0)
                PopulateSingleLineUntilSister(queue1, line1Batches, batchesUntilSister1);
            
            if (line1Batches.Count == 0) 
                break;
            
            var batchesUntilSister2 = GetBatchesUntilSister(line2Batches);
            if (batchesUntilSister2.Count != 0)
                PopulateSingleLineUntilSister(queue2, line2Batches, batchesUntilSister2);
            
            var sisterBatch1 = line1Batches[0];
            var sisterBatch2 = line2Batches[0];
            
            SynchronizeQueues(queue1, queue2, sisterBatch1, sisterBatch2);

            line1Batches.RemoveAt(0);
            line2Batches.RemoveAt(0);
        }

        if (line2Batches.Count != 0)
        {
            var batchesUntilSister2 = GetBatchesUntilSister(line2Batches);
            if (batchesUntilSister2.Count != 0)
                PopulateSingleLineUntilSister(queue2, line2Batches, batchesUntilSister2);
        }

        return queues;
    }
    
    private void PopulateSingleLineUntilSister(TimeTableQueue queue, List<Batch> lineBatches, List<Batch> batchesUntilSister)
    {
        var isFirstBatch = queue.Items.Count == 0;
        var nextStartDate = isFirstBatch ? GetNextStartDate(batchesUntilSister) : GetLastItemEndDate(queue);

        if (!isFirstBatch && IsLastItemPackagingHeightDifferent(queue, batchesUntilSister.FirstOrDefault()))
        {
            queue.Items.Add(CreateAdjustmentItem(nextStartDate));
            nextStartDate += _adjustmentDuration;
        }
        queue.Items.AddRange(
            GenerateSingleLineItems(
                batchesUntilSister,
                isFirstBatch ? 0 : queue.Items.Count(x => x.Type == TimeTableItemType.Batch),
                nextStartDate));
        lineBatches.RemoveRange(0, batchesUntilSister.Count);
    }

    private void SynchronizeQueues(TimeTableQueue queue1, TimeTableQueue queue2, Batch sisterBatch1, Batch sisterBatch2)
    {
        var nextStartDate1 = GetNextStartDate(queue1, sisterBatch1);
        var nextStartDate2 = GetNextStartDate(queue2, sisterBatch2);

        if (nextStartDate1 > nextStartDate2)
        {
            var downtimeDuration = nextStartDate1 - nextStartDate2;
            queue2.Items.Add(CreateDowntimeItem(nextStartDate2, downtimeDuration));
            nextStartDate2 += downtimeDuration;
        }
        else if (nextStartDate1 < nextStartDate2)
        {
            var downtimeDuration = nextStartDate2 - nextStartDate1;
            queue1.Items.Add(CreateDowntimeItem(nextStartDate1, downtimeDuration));
            nextStartDate1 += downtimeDuration;
        }

        if ((queue1.HasItems || queue2.HasItems) && 
            (IsLastItemPackagingHeightDifferent(queue1, sisterBatch1) || IsLastItemPackagingHeightDifferent(queue2, sisterBatch2)))
        {
            // create adjustment only if batch is not first in the queue
            queue1.Items.Add(CreateAdjustmentItem(nextStartDate1)); 
            nextStartDate1 += _adjustmentDuration;
            
            // create adjustment only if batch is not first in the queue
            queue2.Items.Add(CreateAdjustmentItem(nextStartDate2)); 
            nextStartDate2 += _adjustmentDuration;
        }
        
        var batchCount1 = queue1.Items.Count(x => x.Type == TimeTableItemType.Batch);
        var batchCount2 = queue2.Items.Count(x => x.Type == TimeTableItemType.Batch);
        queue1.Items.Add(CreateBatchItem(sisterBatch1, nextStartDate1, batchCount1, batchCount2, sisterBatch2.PackagingCode));
        queue2.Items.Add(CreateBatchItem(sisterBatch2, nextStartDate2, batchCount2, batchCount1, sisterBatch1.PackagingCode));
    }

    private List<TimeTableItem> GenerateSingleLineItems(List<Batch> batches, int batchesCountBeforeLine = 0, DateTimeOffset? start = null)
    {
        var items = new List<TimeTableItem>();
        var nextStart = start ?? GetNextStartDate(batches);

        for (var i = 0; i < batches.Count; i++)
        {
            if (i > 0 && IsLastItemPackagingHeightDifferent(batches[i-1], batches[i]))
            {
                items.Add(CreateAdjustmentItem(nextStart));
                nextStart += _adjustmentDuration;
            }
            
            items.Add(CreateBatchItem(batches[i], nextStart, i + batchesCountBeforeLine, null, null));
            nextStart = items.Last().TimeTableSchedule.End;
        }

        return items;
    }

    private DateTimeOffset GetNextStartDate(List<Batch> batches)
    {
        if (batches.Count > 0 && batches[0].Status == BatchStatus.Washing)
            return (DateTimeOffset)batches[0].WashingStartedAt!;
        
        return _firstStartDate;
    }
    
    private DateTimeOffset GetNextStartDate(TimeTableQueue queue, Batch batch)
    {
        if (batch.Status == BatchStatus.Washing)
            return (DateTimeOffset)batch.WashingStartedAt!;

        return GetLastItemEndDate(queue);
    }

    private static List<Batch> GetBatchesUntilSister(List<Batch> batches)
    {
        return batches.Any(b => b.HasSisterBatch)
            ? batches.TakeWhile(b => !b.HasSisterBatch).ToList()
            : batches.ToList();
    }
        
    private BatchItem CreateBatchItem(Batch batch, DateTimeOffset start, int index, int? sisterBatchIndex, string? sisterPackagingCode)
    {
        var to = batch.Status == BatchStatus.Washing
            ? _firstStartDate + batch.OptimalKitsLeftDuration
            : start + batch.OptimalKitsLeftDuration;
        
        return new BatchItem(
            batch.Id,
            TimeTableItemType.Batch,
            batch.SisterBatchId,
            start,
            to,
            batch.KitCode,
            batch.PackagingCode,
            batch.PackagingHeight,
            sisterPackagingCode,
            batch.KitsFinished,
            batch.KitsCount,
            batch.OptimalKitDuration,
            index,
            sisterBatchIndex,
            batch.Status);
    }

    private static DowntimeItem CreateDowntimeItem(DateTimeOffset start, TimeSpan duration)
    {
        return new DowntimeItem(TimeTableItemType.Downtime, start, start + duration);
    }

    private AdjustmentItem CreateAdjustmentItem(DateTimeOffset start)
    {
        return new AdjustmentItem(TimeTableItemType.WashingMachineAdjustment, start, start + _adjustmentDuration);
    }

    private DateTimeOffset GetLastItemEndDate(TimeTableQueue queue)
    {
        return queue.HasItems ? (DateTimeOffset)queue.GetLastEndDate()! : _firstStartDate;
    }

    private static bool IsLastItemPackagingHeightDifferent(TimeTableQueue queue, Batch? batch)
    {
        return queue.HasItems && queue.GetLastBatchItem?.PackagingHeight != batch?.PackagingHeight;
    }
    
    private static bool IsLastItemPackagingHeightDifferent(Batch lastBatch, Batch newBatch)
    {
        return lastBatch.PackagingHeight != newBatch.PackagingHeight;
    }
}