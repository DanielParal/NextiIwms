using Moq;
using Nexticz.Module.Mmo.Planning.Application.WashingMachineTimeTables;
using Nexticz.Module.Mmo.Planning.Application.WashingMachineTimeTables.Models;
using Nexticz.Module.Mmo.Planning.Domain.BatchEntity;
using Nexticz.Module.Mmo.Planning.Domain.LineQueueEntity;
using Nexticz.Lib.Shared.Time;
using Shouldly;

namespace Nexticz.Module.Mmo.Planning.Tests.Unit.Application.WashingMachineTimeTable;

public class TimeTableSchedulerTests
{
    private readonly TimeSpan _adjustmentDuration = TimeSpan.FromMinutes(15);
    private readonly DateTime _fixedTime = new(2025, 4, 1, 0, 0, 0);
    
    [Fact]
    public void SchedulerNoLine_ShouldReturnEmptyList_WhenLineQueuesAreEmpty()
    {
        var result = CreateSut().Schedule(new List<LineQueue>());
        result.ShouldBeEmpty();
    }
    
    /// Expected Results:
    /// =================
    /// Batch       - 25min - 2025-04-01 00:00:00 - 2025-04-01 00:25:00
    /// Adjustment  - 15min - 2025-04-01 00:25:00 - 2025-04-01 00:40:00
    /// Batch       - 30min - 2025-04-01 00:40:00 - 2025-04-01 01:10:00
    /// Batch       - 16min - 2025-04-01 01:10:00 - 2025-04-01 01:26:00
    [Fact]
    public void ScheduleSingleWithSameHeight_ShouldReturnOneQueueWithItems_WhenSingleLineQueueIsGiven()
    {
        // Arrange
        var batches = new List<Batch>
        {
            CreateBatch(5, TimeSpan.FromMinutes(5)),
            CreateBatch(1, TimeSpan.FromMinutes(30), packagingHeight: 1000),
            CreateBatch(2, TimeSpan.FromMinutes(8), packagingHeight: 1000)
        };
        
        var lineQueues = new List<LineQueue> {CreateLineQueue(batches)};

        // Act
        var result = CreateSut().Schedule(lineQueues);
        
        // Assert
        var line1Items = result.First().Items;
        result.Count.ShouldBe(1);
        line1Items.Count.ShouldBe(batches.Count + 1); // 1 adjustment item
        
        ValidateBatchItem(line1Items[0], _fixedTime, TimeSpan.FromMinutes(25));
        ValidateAdjustmentItem(line1Items[1], line1Items[0].TimeTableSchedule.End);
        ValidateBatchItem(line1Items[2], line1Items[1].TimeTableSchedule.End, TimeSpan.FromMinutes(30));
        ValidateBatchItem(line1Items[3], line1Items[2].TimeTableSchedule.End, TimeSpan.FromMinutes(16));
        
        line1Items[3].TimeTableSchedule.End.ShouldBe(new DateTime(2025, 4, 1, 1, 26, 0));
    }
    
    /// Expected Results:
    /// =================
    /// Batch       - 25min - 2025-04-01 00:00:00 - 2025-04-01 00:25:00
    /// Adjustment  - 15min - 2025-04-01 00:25:00 - 2025-04-01 00:40:00
    /// Batch       - 30min - 2025-04-01 00:40:00 - 2025-04-01 01:10:00
    /// Adjustment  - 15min - 2025-04-01 01:10:00 - 2025-04-01 01:25:00
    /// Batch       - 16min - 2025-04-01 01:25:00 - 2025-04-01 01:41:00
    [Fact]
    public void ScheduleSingle_ShouldReturnOneQueueWithItems_WhenSingleLineQueueIsGiven()
    {
        // Arrange
        var batches = new List<Batch>
        {
            CreateBatch(5, TimeSpan.FromMinutes(5)),
            CreateBatch(1, TimeSpan.FromMinutes(30)),
            CreateBatch(2, TimeSpan.FromMinutes(8))
        };
        
        var lineQueues = new List<LineQueue> {CreateLineQueue(batches)};

        // Act
        var result = CreateSut().Schedule(lineQueues);
        
        // Assert
        var line1Items = result.First().Items;
        result.Count.ShouldBe(1);
        line1Items.Count.ShouldBe(batches.Count + 2); // 2 adjustment items
        
        ValidateBatchItem(line1Items[0], _fixedTime, TimeSpan.FromMinutes(25));
        ValidateAdjustmentItem(line1Items[1], line1Items[0].TimeTableSchedule.End);
        ValidateBatchItem(line1Items[2], line1Items[1].TimeTableSchedule.End, TimeSpan.FromMinutes(30));
        ValidateAdjustmentItem(line1Items[3], line1Items[2].TimeTableSchedule.End);
        ValidateBatchItem(line1Items[4], line1Items[3].TimeTableSchedule.End, TimeSpan.FromMinutes(16));
        
        line1Items[4].TimeTableSchedule.End.ShouldBe(new DateTime(2025, 4, 1, 1, 41, 0));
    }
    
    /// Expected Results:
    /// =================
    /// Batch       - 23hours - 2025-04-01 00:00:00 - 2025-04-01 23:00:00
    /// Adjustment  - 15min   - 2025-04-01 23:00:00 - 2025-04-01 23:15:00
    /// Batch       - 90min   - 2025-04-01 23:15:00 - 2025-04-02 00:45:00
    /// Adjustment  - 15min   - 2025-04-02 00:45:00 - 2025-04-02 01:00:00
    /// Batch       - 25min   - 2025-04-01 01:00:00 - 2025-04-02 01:40:00
    [Fact]
    public void ScheduleSingleCrossMidnight_ShouldReturnOneQueueWithItems_WhenCrossingMidnightAndSingleLineQueueIsGiven()
    {
        // Arrange
        var batches = new List<Batch>
        {
            CreateBatch(1, TimeSpan.FromHours(23)),
            CreateBatch(2, TimeSpan.FromMinutes(45)),
            CreateBatch(4, TimeSpan.FromMinutes(10))
        };
        
        var lineQueues = new List<LineQueue> {CreateLineQueue(batches)};

        // Act
        var result = CreateSut().Schedule(lineQueues);
        
        // Assert
        var line1Items = result.First().Items;
        result.Count.ShouldBe(1);
        line1Items.Count.ShouldBe(batches.Count + 2); // 2 adjustment items
        
        ValidateBatchItem(line1Items[0], _fixedTime, TimeSpan.FromHours(23));
        ValidateAdjustmentItem(line1Items[1], line1Items[0].TimeTableSchedule.End);
        ValidateBatchItem(line1Items[2], line1Items[1].TimeTableSchedule.End, TimeSpan.FromMinutes(90));
        ValidateAdjustmentItem(line1Items[3], line1Items[2].TimeTableSchedule.End);
        ValidateBatchItem(line1Items[4], line1Items[3].TimeTableSchedule.End, TimeSpan.FromMinutes(40));
        
        line1Items[4].TimeTableSchedule.End.ShouldBe(new DateTime(2025, 4, 2, 1, 40, 0));
    }

    /// Lines:
    /// ======
    /// Line 1          Line 2
    /// -------         -------
    /// Batch - 20m     Batch - 40m
    /// Batch - 30m     Batch - 20m
    /// Batch - 10m
    /// 
    /// Expected Results:
    /// =================
    /// Line 1                                                      Line2
    /// ------                                                      -------
    /// Batch       - 2025-04-01 00:00:00 - 2025-04-01 00:20:00     Batch       - 2025-04-01 00:00:00 - 2025-04-01 00:40:00
    /// Adjustment  - 2025-04-01 00:20:00 - 2025-04-01 00:35:00     Adjustment  - 2025-04-01 00:40:00 - 2025-04-01 00:55:00
    /// Batch       - 2025-04-01 00:35:00 - 2025-04-01 01:05:00     Batch       - 2025-04-01 00:55:00 - 2025-04-01 01:15:00
    /// Adjustment  - 2025-04-01 01:05:00 - 2025-04-01 01:20:00
    /// Batch       - 2025-04-01 01:20:00 - 2025-04-01 01:30:00
    [Fact]
    public void ScheduleMultiLineWithNoSisterBatches_ShouldReturnTwoQueuesWithItems_WhenMultilineIsGiven()
    {
        // Arrange
        var batchesLine1 = new List<Batch>
        {
            CreateBatch(2, TimeSpan.FromMinutes(10)),
            CreateBatch(6, TimeSpan.FromMinutes(5)),
            CreateBatch(1, TimeSpan.FromMinutes(10))
        };
        
        var batchesLine2 = new List<Batch>
        {
            CreateBatch(4, TimeSpan.FromMinutes(10)),
            CreateBatch(1, TimeSpan.FromMinutes(20))
        };
        
        var lineQueues = new List<LineQueue> {CreateLineQueue(batchesLine1), CreateLineQueue(batchesLine2)};

        // Act
        var result = CreateSut().Schedule(lineQueues);
        
        // Assert
        
        // Line 1
        var line1Items = result[0].Items;
        result.Count.ShouldBe(2);
        line1Items.Count.ShouldBe(batchesLine1.Count + 2); // 2 adjustment items
        
        ValidateBatchItem(line1Items[0], _fixedTime, TimeSpan.FromMinutes(20));
        ValidateAdjustmentItem(line1Items[1], line1Items[0].TimeTableSchedule.End);
        ValidateBatchItem(line1Items[2], line1Items[1].TimeTableSchedule.End, TimeSpan.FromMinutes(30));
        ValidateAdjustmentItem(line1Items[3], line1Items[2].TimeTableSchedule.End);
        ValidateBatchItem(line1Items[4], line1Items[3].TimeTableSchedule.End, TimeSpan.FromMinutes(10));
        
        line1Items[4].TimeTableSchedule.End.ShouldBe(new DateTime(2025, 4, 1, 1, 30, 0));
        
        // Line 2
        var line2Items = result[1].Items;
        result.Count.ShouldBe(2);
        line2Items.Count.ShouldBe(batchesLine2.Count + 1); // 1 adjustment item
        
        ValidateBatchItem(line2Items[0], _fixedTime, TimeSpan.FromMinutes(40));
        ValidateAdjustmentItem(line2Items[1], line2Items[0].TimeTableSchedule.End);
        ValidateBatchItem(line2Items[2], line2Items[1].TimeTableSchedule.End, TimeSpan.FromMinutes(20));
        
        line2Items[2].TimeTableSchedule.End.ShouldBe(new DateTime(2025, 4, 1, 1, 15, 0));
    }

    /// Lines:
    /// ======
    /// Line 1                      Line 2
    /// -------                     -------
    /// Batch - 20m                 Batch - 40m
    /// Batch - 30m                 Sister Batch 1.2 - 10m
    /// Sister Batch 1.1 - 10m      Batch - 40m
    /// Batch - 20m                 Sister Batch 2.2 - 30m
    /// Sister Batch 2.1 - 20m      
    /// 
    /// Expected Results:
    /// =================
    /// Line 1                                                      Line2
    /// ------                                                      -------
    /// Batch       - 2025-04-01 00:00:00 - 2025-04-01 00:20:00     Batch       - 2025-04-01 00:00:00 - 2025-04-01 00:40:00
    /// Adjustment  - 2025-04-01 00:20:00 - 2025-04-01 00:35:00     Downtime    - 2025-04-01 00:40:00 - 2025-04-01 01:05:00
    /// Adjustment  - 2025-04-01 01:05:00 - 2025-04-01 01:20:00     (25m)
    /// Batch       - 2025-04-01 00:35:00 - 2025-04-01 01:05:00     Adjustment  - 2025-04-01 01:05:00 - 2025-04-01 01:20:00 
    /// Batch S1    - 2025-04-01 01:20:00 - 2025-04-01 01:30:00     Batch S1    - 2025-04-01 01:20:00 - 2025-04-01 01:30:00
    /// Adjustment  - 2025-04-01 01:30:00 - 2025-04-01 01:45:00     Adjustment  - 2025-04-01 01:30:00 - 2025-04-01 01:45:00
    /// Batch       - 2025-04-01 01:45:00 - 2025-04-01 02:05:00     Batch       - 2025-04-01 01:45:00 - 2025-04-01 02:25:00
    /// Downtime    - 2025-04-01 02:05:00 - 2025-04-01 02:25:00     Adjustment  - 2025-04-01 02:25:00 - 2025-04-01 02:40:00
    /// (20m)
    /// Adjustment  - 2025-04-01 02:25:00 - 2025-04-01 02:40:00
    /// Batch S2    - 2025-04-01 02:40:00 - 2025-04-01 03:00:00     Batch S2    - 2025-04-01 02:40:00 - 2025-04-01 03:10:00
    /// 
    [Fact]
    public void ScheduleMultiLineWithSisterBatches_ShouldReturnTwoQueuesWithItems_WhenMultilineIsGiven()
    {
        var sisterBatch11Id = Guid.NewGuid();
        var sisterBatch12Id = Guid.NewGuid();
        
        var sisterBatch21Id = Guid.NewGuid();
        var sisterBatch22Id = Guid.NewGuid();

        // Arrange
        var batchesLine1 = new List<Batch>
        {
            CreateBatch(2, TimeSpan.FromMinutes(10)),
            CreateBatch(3, TimeSpan.FromMinutes(10)),
            CreateBatch(1, TimeSpan.FromMinutes(10), sisterBatch12Id),
            CreateBatch(1, TimeSpan.FromMinutes(20)),
            CreateBatch(1, TimeSpan.FromMinutes(20), sisterBatch22Id)
        };
        
        var batchesLine2 = new List<Batch>
        {
            CreateBatch(4, TimeSpan.FromMinutes(10)),
            CreateBatch(1, TimeSpan.FromMinutes(10), sisterBatch11Id),
            CreateBatch(1, TimeSpan.FromMinutes(40)),
            CreateBatch(1, TimeSpan.FromMinutes(30), sisterBatch21Id)
        };
        
        var lineQueues = new List<LineQueue> {CreateLineQueue(batchesLine1), CreateLineQueue(batchesLine2)};

        // Act
        var result = CreateSut().Schedule(lineQueues);
        
        // Assert
        
        // Line 1
        var line1Items = result[0].Items;
        result.Count.ShouldBe(2);
        line1Items.Count.ShouldBe(10); 
        
        ValidateBatchItem(line1Items[0], _fixedTime, TimeSpan.FromMinutes(20));
        ValidateAdjustmentItem(line1Items[1], line1Items[0].TimeTableSchedule.End);
        ValidateBatchItem(line1Items[2], line1Items[1].TimeTableSchedule.End, TimeSpan.FromMinutes(30));
        ValidateAdjustmentItem(line1Items[3], line1Items[2].TimeTableSchedule.End);
        ValidateBatchItem(line1Items[4], line1Items[3].TimeTableSchedule.End, TimeSpan.FromMinutes(10));
        ValidateAdjustmentItem(line1Items[5], line1Items[4].TimeTableSchedule.End);
        ValidateBatchItem(line1Items[6], line1Items[5].TimeTableSchedule.End, TimeSpan.FromMinutes(20));
        ValidateDowntimeItem(line1Items[7], line1Items[6].TimeTableSchedule.End, line1Items[8].TimeTableSchedule.Start);
        ValidateAdjustmentItem(line1Items[8], line1Items[7].TimeTableSchedule.End);
        ValidateBatchItem(line1Items[9], line1Items[8].TimeTableSchedule.End, TimeSpan.FromMinutes(20));
        
        line1Items[9].TimeTableSchedule.End.ShouldBe(new DateTime(2025, 4, 1, 3, 0, 0));
        
         // Line 1
        var line2Items = result[1].Items;
        result.Count.ShouldBe(2);
        line2Items.Count.ShouldBe(8); 
        
        ValidateBatchItem(line2Items[0], _fixedTime, TimeSpan.FromMinutes(40));
        ValidateDowntimeItem(line2Items[1], line2Items[0].TimeTableSchedule.End, line2Items[2].TimeTableSchedule.Start);
        ValidateAdjustmentItem(line2Items[2], line2Items[1].TimeTableSchedule.End);
        ValidateBatchItem(line2Items[3], line2Items[2].TimeTableSchedule.End, TimeSpan.FromMinutes(10));
        ValidateAdjustmentItem(line2Items[4], line2Items[3].TimeTableSchedule.End);
        ValidateBatchItem(line2Items[5], line2Items[4].TimeTableSchedule.End, TimeSpan.FromMinutes(40));
        ValidateAdjustmentItem(line2Items[6], line2Items[5].TimeTableSchedule.End);
        ValidateBatchItem(line2Items[7], line2Items[6].TimeTableSchedule.End, TimeSpan.FromMinutes(30));
        
        line2Items[7].TimeTableSchedule.End.ShouldBe(new DateTime(2025, 4, 1, 3, 10, 0));
    }

    /// Lines:
    /// ======
    /// Line 1                      Line 2
    /// -------                     -------
    /// Sister Batch 1.1 - 20m      Sister Batch 1.2 - 40m
    /// Batch - 30m                 Batch - 40m
    /// Batch - 50m                 Batch - 10m
    /// 
    /// Expected Results:
    /// =================
    /// Line 1                                                      Line2
    /// ------                                                      -------
    /// Batch S1    - 2025-04-01 00:00:00 - 2025-04-01 00:20:00     Batch S1    - 2025-04-01 00:00:00 - 2025-04-01 00:40:00
    /// Adjustment  - 2025-04-01 00:20:00 - 2025-04-01 00:35:00     Adjustment  - 2025-04-01 00:40:00 - 2025-04-01 00:55:00
    /// Batch       - 2025-04-01 00:35:00 - 2025-04-01 01:05:00     Batch       - 2025-04-01 00:55:00 - 2025-04-01 01:35:00
    /// Adjustment  - 2025-04-01 01:05:00 - 2025-04-01 01:20:00     Adjustment  - 2025-04-01 01:35:00 - 2025-04-01 01:50:00 
    /// Batch       - 2025-04-01 01:20:00 - 2025-04-01 02:10:00     Batch       - 2025-04-01 01:50:00 - 2025-04-01 02:00:00
    /// 
    [Fact]
    public void ScheduleMultiLineStartingWithSisterBatches_ShouldReturnTwoQueuesWithItems_WhenMultilineIsGiven()
    {
        var sisterBatch11Id = Guid.NewGuid();
        var sisterBatch12Id = Guid.NewGuid();
        
        // Arrange
        var batchesLine1 = new List<Batch>
        {
            CreateBatch(1, TimeSpan.FromMinutes(20), sisterBatch11Id),
            CreateBatch(1, TimeSpan.FromMinutes(30)),
            CreateBatch(1, TimeSpan.FromMinutes(50))
        };
        
        var batchesLine2 = new List<Batch>
        {
            CreateBatch(1, TimeSpan.FromMinutes(40), sisterBatch12Id),
            CreateBatch(1, TimeSpan.FromMinutes(40)),
            CreateBatch(1, TimeSpan.FromMinutes(10))
        };
        
        var lineQueues = new List<LineQueue> {CreateLineQueue(batchesLine1), CreateLineQueue(batchesLine2)};

        // Act
        var result = CreateSut().Schedule(lineQueues);
        
        // Assert
        
        // Line 1
        var line1Items = result[0].Items;
        result.Count.ShouldBe(2);
        line1Items.Count.ShouldBe(5); 
        
        ValidateBatchItem(line1Items[0], _fixedTime, TimeSpan.FromMinutes(20));
        ValidateAdjustmentItem(line1Items[1], line1Items[0].TimeTableSchedule.End);
        ValidateBatchItem(line1Items[2], line1Items[1].TimeTableSchedule.End, TimeSpan.FromMinutes(30));
        ValidateAdjustmentItem(line1Items[3], line1Items[2].TimeTableSchedule.End);
        ValidateBatchItem(line1Items[4], line1Items[3].TimeTableSchedule.End, TimeSpan.FromMinutes(50));
        
        line1Items[4].TimeTableSchedule.End.ShouldBe(new DateTime(2025, 4, 1, 2, 10, 0));
        
         // Line 1
        var line2Items = result[1].Items;
        result.Count.ShouldBe(2);
        line2Items.Count.ShouldBe(5); 
        
        ValidateBatchItem(line2Items[0], _fixedTime, TimeSpan.FromMinutes(40));
        ValidateAdjustmentItem(line2Items[1], line2Items[0].TimeTableSchedule.End);
        ValidateBatchItem(line2Items[2], line2Items[1].TimeTableSchedule.End, TimeSpan.FromMinutes(40));
        ValidateAdjustmentItem(line2Items[3], line2Items[2].TimeTableSchedule.End);
        ValidateBatchItem(line2Items[4], line2Items[3].TimeTableSchedule.End, TimeSpan.FromMinutes(10));
        
        line2Items[4].TimeTableSchedule.End.ShouldBe(new DateTime(2025, 4, 1, 2, 00, 0));
    }
    
    /// Lines:
    /// ======
    /// Line 1                      Line 2
    /// -------                     -------
    /// Sister Batch 1.1 - 20m      Sister Batch 1.2 - 40m
    /// Batch - 30m
    /// Batch - 50m
    /// 
    /// Expected Results:
    /// =================
    /// Line 1                                                      Line2
    /// ------                                                      -------
    /// Batch S1    - 2025-04-01 00:00:00 - 2025-04-01 00:20:00     Batch S1    - 2025-04-01 00:00:00 - 2025-04-01 00:40:00
    /// Adjustment  - 2025-04-01 00:20:00 - 2025-04-01 00:35:00
    /// Batch       - 2025-04-01 00:35:00 - 2025-04-01 01:05:00     
    /// Adjustment  - 2025-04-01 01:05:00 - 2025-04-01 01:20:00     
    /// Batch       - 2025-04-01 01:20:00 - 2025-04-01 02:10:00     
    /// 
    [Fact]
    public void ScheduleMultiLineStartingWithSisterBatchesAndNoOtherBatch_ShouldReturnTwoQueuesWithItems_WhenMultilineIsGiven()
    {
        var sisterBatch11Id = Guid.NewGuid();
        var sisterBatch12Id = Guid.NewGuid();
        
        // Arrange
        var batchesLine1 = new List<Batch>
        {
            CreateBatch(1, TimeSpan.FromMinutes(20), sisterBatch11Id),
            CreateBatch(1, TimeSpan.FromMinutes(30)),
            CreateBatch(1, TimeSpan.FromMinutes(50))
        };
        
        var batchesLine2 = new List<Batch>
        {
            CreateBatch(1, TimeSpan.FromMinutes(40), sisterBatch12Id)
        };
        
        var lineQueues = new List<LineQueue> {CreateLineQueue(batchesLine1), CreateLineQueue(batchesLine2)};

        // Act
        var result = CreateSut().Schedule(lineQueues);
        
        // Assert
        
        // Line 1
        var line1Items = result[0].Items;
        result.Count.ShouldBe(2);
        line1Items.Count.ShouldBe(5); 
        
        ValidateBatchItem(line1Items[0], _fixedTime, TimeSpan.FromMinutes(20));
        ValidateAdjustmentItem(line1Items[1], line1Items[0].TimeTableSchedule.End);
        ValidateBatchItem(line1Items[2], line1Items[1].TimeTableSchedule.End, TimeSpan.FromMinutes(30));
        ValidateAdjustmentItem(line1Items[3], line1Items[2].TimeTableSchedule.End);
        ValidateBatchItem(line1Items[4], line1Items[3].TimeTableSchedule.End, TimeSpan.FromMinutes(50));
        
        line1Items[4].TimeTableSchedule.End.ShouldBe(new DateTime(2025, 4, 1, 2, 10, 0));
        
         // Line 2
        var line2Items = result[1].Items;
        result.Count.ShouldBe(2);
        line2Items.Count.ShouldBe(1); 
        
        ValidateBatchItem(line2Items[0], _fixedTime, TimeSpan.FromMinutes(40));
        
        line2Items[0].TimeTableSchedule.End.ShouldBe(new DateTime(2025, 4, 1, 0, 40, 0));
    }

    /// Lines:
    /// ======
    /// Line 1                      Line 2
    /// -------                     -------
    /// Batch - 30m
    /// Batch - 50m
    /// 
    /// Expected Results:
    /// =================
    /// Line 1                                                      Line2
    /// ------                                                      -------
    /// Batch S1    - 2025-04-01 00:00:00 - 2025-04-01 00:30:00     
    /// Adjustment  - 2025-04-01 00:30:00 - 2025-04-01 00:45:00
    /// Batch       - 2025-04-01 00:45:00 - 2025-04-01 01:35:00   
    /// 
    [Fact]
    public void ScheduleMultiLineWithItemsOnlyInOneLine_ShouldReturnOneLineWithItemsAndOneEmpty_WhenOneLineIsEmpty()
    {
        // Arrange
        var batchesLine1 = new List<Batch>
        {
            CreateBatch(2, TimeSpan.FromMinutes(15)),
            CreateBatch(1, TimeSpan.FromMinutes(50))
        };

        var batchesLine2 = new List<Batch>();
        
        var lineQueues = new List<LineQueue> {CreateLineQueue(batchesLine1), CreateLineQueue(batchesLine2)};

        // Act
        var result = CreateSut().Schedule(lineQueues);
        
        // Assert
        
        // Line 1
        var line1Items = result[0].Items;
        result.Count.ShouldBe(2);
        line1Items.Count.ShouldBe(3); 
        
        ValidateBatchItem(line1Items[0], _fixedTime, TimeSpan.FromMinutes(30));
        ValidateAdjustmentItem(line1Items[1], line1Items[0].TimeTableSchedule.End);
        ValidateBatchItem(line1Items[2], line1Items[1].TimeTableSchedule.End, TimeSpan.FromMinutes(50));
        
        line1Items[2].TimeTableSchedule.End.ShouldBe(new DateTime(2025, 4, 1, 1, 35, 0));
        
        // Line 2
        var line2Items = result[1].Items;
        result.Count.ShouldBe(2);
        line2Items.Count.ShouldBe(0);
    }

    /// Lines:
    /// ======
    /// Line 1                      Line 2
    /// -------                     -------
    /// Batch    - 30m              Batch S1 - 25m
    /// Batch S1 - 15m              Batch S2 - 40m
    /// Batch S2 - 50m              Batch    - 40m
    /// 
    /// Expected Results:
    /// =================
    /// Line 1                                                          Line2
    /// ------                                                          -------
    /// Batch           - 2025-04-01 00:00:00 - 2025-04-01 00:30:00     Downtime (30m)  - 2025-04-01 00:00:00 - 2025-04-01 00:30:00
    /// Adjustment      - 2025-04-01 00:30:00 - 2025-04-01 00:45:00     Adjustment      - 2025-04-01 00:30:00 - 2025-04-01 00:45:00
    /// Batch S1        - 2025-04-01 00:45:00 - 2025-04-01 01:00:00     Batch S1        - 2025-04-01 00:45:00 - 2025-04-01 01:10:00
    /// Downtime (10m)  - 2025-04-01 01:00:00 - 2025-04-01 01:10:00     
    /// Adjustment      - 2025-04-01 01:10:00 - 2025-04-01 01:25:00     Adjustment      - 2025-04-01 01:10:00 - 2025-04-01 01:25:00
    /// Batch S2        - 2025-04-01 01:25:00 - 2025-04-01 02:15:00     Batch S2        - 2025-04-01 01:25:00 - 2025-04-01 02:05:00
    ///                                                                 Adjustment      - 2025-04-01 02:05:00 - 2025-04-01 02:20:00
    ///                                                                 Batch           - 2025-04-01 02:20:00 - 2025-04-01 03:00:00
    /// 
    [Fact]
    public void ScheduleMultiLineWithStartingSisterInSecondLine_ShouldReturnTwoLinesWithItems_WhenSecondLineIsDelayed()
    {
        // Arrange
        var sisterBatch11Id = Guid.NewGuid();
        var sisterBatch12Id = Guid.NewGuid();
        
        var sisterBatch21Id = Guid.NewGuid();
        var sisterBatch22Id = Guid.NewGuid();
        
        var batchesLine1 = new List<Batch>
        {
            CreateBatch(1, TimeSpan.FromMinutes(30)),
            CreateBatch(1, TimeSpan.FromMinutes(15), sisterBatch12Id),
            CreateBatch(1, TimeSpan.FromMinutes(50), sisterBatch22Id)
        };
        
        var batchesLine2 = new List<Batch>
        {
            CreateBatch(1, TimeSpan.FromMinutes(25), sisterBatch11Id),
            CreateBatch(1, TimeSpan.FromMinutes(40), sisterBatch21Id),
            CreateBatch(1, TimeSpan.FromMinutes(40))
        };
        
        var lineQueues = new List<LineQueue> {CreateLineQueue(batchesLine1), CreateLineQueue(batchesLine2)};

        // Act
        var result = CreateSut().Schedule(lineQueues);
        
        // Assert
        
        // Line 1
        var line1Items = result[0].Items;
        result.Count.ShouldBe(2);
        line1Items.Count.ShouldBe(6); 
        
        ValidateBatchItem(line1Items[0], _fixedTime, TimeSpan.FromMinutes(30));
        ValidateAdjustmentItem(line1Items[1], line1Items[0].TimeTableSchedule.End);
        ValidateBatchItem(line1Items[2], line1Items[1].TimeTableSchedule.End, TimeSpan.FromMinutes(15));
        ValidateDowntimeItem(line1Items[3], line1Items[2].TimeTableSchedule.End, line1Items[4].TimeTableSchedule.Start);
        ValidateAdjustmentItem(line1Items[4], line1Items[3].TimeTableSchedule.End);
        ValidateBatchItem(line1Items[5], line1Items[4].TimeTableSchedule.End, TimeSpan.FromMinutes(50));
        
        line1Items[5].TimeTableSchedule.End.ShouldBe(new DateTime(2025, 4, 1, 2, 15, 0));
        
        // Line 2
        var line2Items = result[1].Items;
        result.Count.ShouldBe(2);
        line2Items.Count.ShouldBe(7); 
        
        ValidateDowntimeItem(line2Items[0], _fixedTime, line2Items[1].TimeTableSchedule.Start);
        ValidateAdjustmentItem(line2Items[1], line2Items[0].TimeTableSchedule.End);
        ValidateBatchItem(line2Items[2], line2Items[1].TimeTableSchedule.End, TimeSpan.FromMinutes(25));
        ValidateAdjustmentItem(line2Items[3], line2Items[2].TimeTableSchedule.End);
        ValidateBatchItem(line2Items[4], line2Items[3].TimeTableSchedule.End, TimeSpan.FromMinutes(40));
        ValidateAdjustmentItem(line2Items[5], line2Items[4].TimeTableSchedule.End);
        ValidateBatchItem(line2Items[6], line2Items[5].TimeTableSchedule.End, TimeSpan.FromMinutes(40));
        
        line2Items[6].TimeTableSchedule.End.ShouldBe(new DateTime(2025, 4, 1, 3, 0, 0));
    }

    /// Lines:
    /// ======
    /// Line 1                      Line 2
    /// -------                     -------
    /// Batch S1 - 10m              Batch    - 25m
    /// Batch    - 35m              Batch S1 - 20m
    ///                             Batch    - 40m
    /// 
    /// Expected Results:
    /// =================
    /// Line 1                                                          Line2
    /// ------                                                          -------
    /// Downtime (25m)  - 2025-04-01 00:00:00 - 2025-04-01 00:25:00     Batch           - 2025-04-01 00:00:00 - 2025-04-01 00:25:00
    /// Adjustment      - 2025-04-01 00:25:00 - 2025-04-01 00:40:00     Adjustment      - 2025-04-01 00:25:00 - 2025-04-01 00:40:00
    /// Batch S1        - 2025-04-01 00:40:00 - 2025-04-01 00:50:00     Batch S1        - 2025-04-01 00:40:00 - 2025-04-01 01:00:00
    /// Adjustment      - 2025-04-01 00:50:00 - 2025-04-01 01:05:00     Adjustment      - 2025-04-01 01:00:00 - 2025-04-01 01:15:00
    /// Batch           - 2025-04-01 01:05:00 - 2025-04-01 01:40:00     Batch           - 2025-04-01 01:15:00 - 2025-04-01 01:55:00
    ///
    [Fact]
    public void ScheduleMultiLineWithStartingSisterInFirstLine_ShouldReturnTwoLinesWithItems_WhenSecondLineIsDelayed()
    {
        // Arrange
        var sisterBatch11Id = Guid.NewGuid();
        var sisterBatch12Id = Guid.NewGuid();
        
        var batchesLine1 = new List<Batch>
        {
            CreateBatch(1, TimeSpan.FromMinutes(10), sisterBatch12Id),
            CreateBatch(1, TimeSpan.FromMinutes(35))
        };
        
        var batchesLine2 = new List<Batch>
        {
            CreateBatch(1, TimeSpan.FromMinutes(25)),
            CreateBatch(1, TimeSpan.FromMinutes(20), sisterBatch11Id),
            CreateBatch(1, TimeSpan.FromMinutes(40))
        };
        
        var lineQueues = new List<LineQueue> {CreateLineQueue(batchesLine1), CreateLineQueue(batchesLine2)};

        // Act
        var result = CreateSut().Schedule(lineQueues);
        
        // Assert
        
        // Line 1
        var line1Items = result[0].Items;
        result.Count.ShouldBe(2);
        line1Items.Count.ShouldBe(5); 
        
        ValidateDowntimeItem(line1Items[0], _fixedTime, line1Items[1].TimeTableSchedule.Start);
        ValidateAdjustmentItem(line1Items[1], line1Items[0].TimeTableSchedule.End);
        ValidateBatchItem(line1Items[2], line1Items[1].TimeTableSchedule.End, TimeSpan.FromMinutes(10));
        ValidateAdjustmentItem(line1Items[3], line1Items[2].TimeTableSchedule.End);
        ValidateBatchItem(line1Items[4], line1Items[3].TimeTableSchedule.End, TimeSpan.FromMinutes(35));
        
        line1Items[4].TimeTableSchedule.End.ShouldBe(new DateTime(2025, 4, 1, 1, 40, 0));
        
        // Line 2
        var line2Items = result[1].Items;
        result.Count.ShouldBe(2);
        line2Items.Count.ShouldBe(5); 
        
        ValidateBatchItem(line2Items[0], _fixedTime, TimeSpan.FromMinutes(25));
        ValidateAdjustmentItem(line2Items[1], line2Items[0].TimeTableSchedule.End);
        ValidateBatchItem(line2Items[2], line2Items[1].TimeTableSchedule.End, TimeSpan.FromMinutes(20));
        ValidateAdjustmentItem(line2Items[3], line2Items[2].TimeTableSchedule.End);
        ValidateBatchItem(line2Items[4], line2Items[3].TimeTableSchedule.End, TimeSpan.FromMinutes(40));
        
        line2Items[4].TimeTableSchedule.End.ShouldBe(new DateTime(2025, 4, 1, 1, 55, 0));
    }
    
    /// Expected Results:
    /// =================
    /// Batch Washing 2/7   - 25min - 2025-04-01 00:00:00 - 2025-04-01 00:25:00
    /// Adjustment          - 15min - 2025-04-01 00:25:00 - 2025-04-01 00:40:00
    /// Batch               - 30min - 2025-04-01 00:40:00 - 2025-04-01 01:10:00
    /// Adjustment          - 15min - 2025-04-01 01:10:00 - 2025-04-01 01:25:00
    /// Batch               - 25min - 2025-04-01 01:25:00 - 2025-04-01 01:41:00
    [Fact]
    public void ScheduleSingleLineWithWashing_ShouldReturnOneQueueWithItems_WhenFirstBatchIsInWashingAndStarted10MinutesLater()
    {
        
        var firstBatch = CreateBatch(7, TimeSpan.FromMinutes(5));
        // Arrange
        var batches = new List<Batch>
        {
            firstBatch,
            CreateBatch(1, TimeSpan.FromMinutes(30)),
            CreateBatch(2, TimeSpan.FromMinutes(8))
        };

        var lineQueues = new List<LineQueue> {CreateLineQueue(batches)};
        
        firstBatch.StartWashing(_fixedTime);
        firstBatch.FinishKit(_fixedTime + TimeSpan.FromMinutes(10));
        firstBatch.FinishKit(_fixedTime + TimeSpan.FromMinutes(20));
        
        // Act
        var result = CreateSut().Schedule(lineQueues);
        
        // Assert
        var line1Items = result.First().Items;
        result.Count.ShouldBe(1);
        line1Items.Count.ShouldBe(batches.Count + 2); // 2 adjustment items
        
        ValidateBatchItem(line1Items[0], _fixedTime, 5 * TimeSpan.FromMinutes(5));
        ValidateAdjustmentItem(line1Items[1], line1Items[0].TimeTableSchedule.End);
        ValidateBatchItem(line1Items[2], line1Items[1].TimeTableSchedule.End, TimeSpan.FromMinutes(30));
        ValidateAdjustmentItem(line1Items[3], line1Items[2].TimeTableSchedule.End);
        ValidateBatchItem(line1Items[4], line1Items[3].TimeTableSchedule.End, TimeSpan.FromMinutes(16));
        
        line1Items[4].TimeTableSchedule.End.ShouldBe(new DateTime(2025, 4, 1, 1, 41, 0));
    }
    
    /// Lines:
    /// ======
    /// Line 1                      Line 2
    /// -------                     -------
    /// Sister Batch 1.1 Washing - 20m      Sister Batch Washing 1.2 - 40m
    /// Batch - 30m
    /// 
    /// Expected Results:
    /// =================
    /// Line 1                                                                  Line2
    /// ------                                                                  -------
    /// Batch S1 washing 1/2    - 2025-04-01 00:00:00 - 2025-04-01 00:20:00     Batch S1 washing 1/2   - 2025-04-01 00:00:00 - 2025-04-01 00:40:00
    /// Adjustment              - 2025-04-01 00:20:00 - 2025-04-01 00:35:00
    /// Batch                   - 2025-04-01 00:35:00 - 2025-04-01 01:05:00    
    /// 
    [Fact]
    public void ScheduleMultiLineStartingWithWashingSisterBatches_ShouldReturnTwoQueuesWithItems_WhenFirstSisterBatchesAreInWashing()
    {
        var sisterBatch11Id = Guid.NewGuid();
        var sisterBatch12Id = Guid.NewGuid();
        
        var sisterBatch11 = CreateBatch(2, TimeSpan.FromMinutes(20), sisterBatch11Id);
        // Arrange
        var batchesLine1 = new List<Batch>
        {
            sisterBatch11,
            CreateBatch(1, TimeSpan.FromMinutes(30)),
            CreateBatch(1, TimeSpan.FromMinutes(50))
        };
        
        var sisterBatch12 = CreateBatch(2, TimeSpan.FromMinutes(40), sisterBatch12Id);
        var batchesLine2 = new List<Batch>
        {
            sisterBatch12
        };
        
        var lineQueues = new List<LineQueue> {CreateLineQueue(batchesLine1), CreateLineQueue(batchesLine2)};
        
        sisterBatch11.StartWashing(_fixedTime);
        sisterBatch11.FinishKit(_fixedTime + TimeSpan.FromMinutes(5));

        sisterBatch12.StartWashing(_fixedTime);
        sisterBatch12.FinishKit(_fixedTime + TimeSpan.FromMinutes(5));

        // Act
        var result = CreateSut().Schedule(lineQueues);
        
        // Assert
        
        // Line 1
        var line1Items = result[0].Items;
        result.Count.ShouldBe(2);
        line1Items.Count.ShouldBe(5); 
        
        ValidateBatchItem(line1Items[0], _fixedTime,  TimeSpan.FromMinutes(20));
        ValidateAdjustmentItem(line1Items[1], line1Items[0].TimeTableSchedule.End);
        ValidateBatchItem(line1Items[2], line1Items[1].TimeTableSchedule.End, TimeSpan.FromMinutes(30));
        
        line1Items[4].TimeTableSchedule.End.ShouldBe(new DateTime(2025, 4, 1, 2, 10, 0));
        
         // Line 2
        var line2Items = result[1].Items;
        result.Count.ShouldBe(2);
        line2Items.Count.ShouldBe(1); 
        
        ValidateBatchItem(line2Items[0], _fixedTime, TimeSpan.FromMinutes(40));
        
        line2Items[0].TimeTableSchedule.End.ShouldBe(new DateTime(2025, 4, 1, 0, 40, 0));
    }
    
    private static void ValidateBatchItem(TimeTableItem item, DateTimeOffset expectedStart, TimeSpan batchLeftDuration)
    {
        item.ShouldBeOfType<BatchItem>();
        item.TimeTableSchedule.Start.ShouldBe(expectedStart);
        item.TimeTableSchedule.End.ShouldBe(item.TimeTableSchedule.Start + batchLeftDuration);
    }
    
    private void ValidateAdjustmentItem(TimeTableItem item, DateTimeOffset expectedStart)
    {
        item.ShouldBeOfType<AdjustmentItem>();
        item.TimeTableSchedule.Start.ShouldBe(expectedStart);
        item.TimeTableSchedule.End.ShouldBe(item.TimeTableSchedule.Start + _adjustmentDuration);
    }

    private static void ValidateDowntimeItem(TimeTableItem item, DateTimeOffset expectedStart, DateTimeOffset expectedEnd)
    {
        item.ShouldBeOfType<DowntimeItem>();
        item.TimeTableSchedule.Start.ShouldBe(expectedStart);
        item.TimeTableSchedule.End.ShouldBe(expectedEnd);
    }
    
    private TimeTableScheduler CreateSut()
    {
        var clockMock = new Mock<IClock>();
        clockMock.Setup(c => c.UtcNowOffset).Returns(_fixedTime);
        return new TimeTableScheduler(_adjustmentDuration, clockMock.Object);
    }

    private static LineQueue CreateLineQueue(List<Batch> batches)
    {
        var line1 = new LineQueue(Guid.NewGuid().ToString(), true, []);

        foreach (var batch in batches)
        {
            line1.AddBatch(batch);
        }
        
        return line1;
    }

    private static int _alwaysDifferentPackagingHeight = 1;
    private static Batch CreateBatch(
        int kitsCount,
        TimeSpan optimalKitDuration,
        Guid? sisterBatchId = null,
        int? packagingHeight = null)
    {
        return new Batch(
            sisterBatchId, 
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(), 
            Guid.NewGuid().ToString(), 
            kitsCount, 
            Guid.NewGuid().ToString(), 
            packagingHeight ?? _alwaysDifferentPackagingHeight ++,
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            optimalKitDuration);
    }
}