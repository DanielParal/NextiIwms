using ErrorOr;
using Nexticz.Module.Mmo.Planning.Domain.BatchEntity;
using Nexticz.Module.Mmo.Planning.Domain.LineQueueEntity;
using Shouldly;

namespace Nexticz.Module.Mmo.Planning.Tests.Unit.Domain;

public class LineQueueTests
{
    [Fact]
    public void Constructor_ShouldInitializeLineQueue_WhenValidArguments()
    {
        const string washingMachineLineCode = "LINE123";
        
        var lineQueue = new LineQueue(washingMachineLineCode, true, []);
        
        lineQueue.ShouldNotBeNull();
        lineQueue.WashingMachineLineCode.ShouldBe(washingMachineLineCode);
        lineQueue.Batches.ShouldBeEmpty();
    }

    [Fact]
    public void Constructor_ShouldThrowException_WhenInvalidWashingMachineLineCode()
    {
        var invalidCode = string.Empty;
        
        var exception = Should.Throw<ArgumentException>(() => new LineQueue(invalidCode, true, []));
        exception.Message.ShouldBe("WashingMachineLineCode cannot be null or empty. (Parameter 'washingMachineLineCode')");
    }

    [Fact]
    public void AddBatch_ShouldAddBatch_WhenValid()
    {
        var lineQueue = new LineQueue("LINE123", true, []);
        var batch = CreateNewBatch();
        
        var result = lineQueue.AddBatch(batch);
        
        result.Value.ShouldBe(Result.Success);
        lineQueue.Batches.ShouldContain(batch);
    }
    
    [Fact]
    public void AddBatch_ShouldAddBatchToLastPosition_WhenMoreBatches()
    {
        var lineQueue = new LineQueue("LINE123",  true, []);
        
        lineQueue.AddBatch(CreateNewBatch());
        lineQueue.AddBatch(CreateNewBatch());
        var lastBatch = CreateNewBatch();
        lineQueue.AddBatch(lastBatch);
        
        lineQueue.Batches.Count.ShouldBe(3);
        lineQueue.Batches.Last().ShouldBe(lastBatch);
    }

    [Fact]
    public void RemoveBatch_ShouldRemoveBatch_WhenValid()
    {
        var lineQueue = new LineQueue("LINE123", true, []);
        var batch = CreateNewBatch();
        lineQueue.AddBatch(batch);
        
        var result = lineQueue.RemoveBatch(batch.Id);
        
        result.Value.ShouldBe(Result.Success);
        lineQueue.Batches.ShouldNotContain(batch);
    }
    
    [Fact]
    public void RemoveBatch_ShouldRemoveBatchMoveOtherBatches_WhenValidRemove()
    {
        var lineQueue = new LineQueue("LINE123", true, []);
        lineQueue.AddBatch(CreateNewBatch());
        var secondToBeDeleted = CreateNewBatch();
        lineQueue.AddBatch(secondToBeDeleted);
        var thirdBatchWhichWillBecomeSecondAfterDelete = CreateNewBatch();
        lineQueue.AddBatch(thirdBatchWhichWillBecomeSecondAfterDelete);
        lineQueue.AddBatch(CreateNewBatch());
        
        var result = lineQueue.RemoveBatch(secondToBeDeleted.Id);
        
        result.Value.ShouldBe(Result.Success);
        lineQueue.Batches.Count.ShouldBe(3);
        lineQueue.Batches.ElementAt(1).ShouldBe(thirdBatchWhichWillBecomeSecondAfterDelete);
    }

    [Fact]
    public void MoveBatch_ShouldMoveBatch_WhenValid()
    {
        var lineQueue = new LineQueue("LINE123", true, []);
        lineQueue.AddBatch(CreateNewBatch());
        lineQueue.AddBatch(CreateNewBatch());
        var batchToBeMoved = CreateNewBatch();
        lineQueue.AddBatch(batchToBeMoved);
        lineQueue.AddBatch(CreateNewBatch());
        
        var result = lineQueue.MoveBatch(batchToBeMoved.Id, 0);
        
        result.Value.ShouldBe(Result.Success);
        lineQueue.Batches.ElementAt(0).ShouldBe(batchToBeMoved);
    }
    
    [Fact]
    public void MoveBatch_ShouldFail_WhenNonExistingBatchId()
    {
        var lineQueue = new LineQueue("LINE123", true, []);
        lineQueue.AddBatch(CreateNewBatch());
        lineQueue.AddBatch(CreateNewBatch());
        
        var result = lineQueue.MoveBatch(Guid.NewGuid(), 0);
        
        result.IsError.ShouldBeTrue();
        result.FirstError.Code.ShouldBe(LineQueueErrors.ValidationBatchNotFoundInTheList.Code);
    }
    
    [Fact]
    public void MoveBatch_ShouldFail_WhenIndexOutOfRange()
    {
        var lineQueue = new LineQueue("LINE123", true, []);
        lineQueue.AddBatch(CreateNewBatch());
        lineQueue.AddBatch(CreateNewBatch());
        var batchToBeMoved = CreateNewBatch();
        lineQueue.AddBatch(batchToBeMoved);
        
        var result = lineQueue.MoveBatch(batchToBeMoved.Id, 4);
        
        result.IsError.ShouldBeTrue();
        result.FirstError.Code.ShouldBe(LineQueueErrors.ValidationBatchCannotBeMovedOutOfRange.Code);
    }
    
    [Fact]
    public void MoveBatch_ShouldFail_WhenFirstBatchIsInWashingStatus()
    {
        var lineQueue = new LineQueue("LINE123", true, []);
        var firstWashingBatch = CreateNewBatch();
        lineQueue.AddBatch(firstWashingBatch);
        firstWashingBatch.StartWashing(DateTimeOffset.Now);
        lineQueue.AddBatch(CreateNewBatch());
        var batchToBeMoved = CreateNewBatch();
        lineQueue.AddBatch(batchToBeMoved);
        
        var result = lineQueue.MoveBatch(batchToBeMoved.Id, 0);
        
        result.IsError.ShouldBeTrue();
        result.FirstError.Code.ShouldBe(LineQueueErrors.ValidationBatchCannotBeMovedBeforeBatchWichIsWashing.Code);
    }
    
    [Fact]
    public void StartWashingBatch_ShouldStartSecondBatchAndFinishFirstWashingBatch_WhenValid()
    {
        var lineQueue = new LineQueue("LINE123", true, []);
        var firstBatch = CreateNewBatch();
        lineQueue.AddBatch(firstBatch);
        firstBatch.StartWashing(DateTimeOffset.Now);
        var secondBatch = CreateNewBatch();
        lineQueue.AddBatch(secondBatch);
        lineQueue.AddBatch(CreateNewBatch());
        lineQueue.AddBatch(CreateNewBatch());
        
        var pastTime = DateTimeOffset.Now - TimeSpan.FromMinutes(10);
        var result = lineQueue.StartBatchWashing(secondBatch.Id, pastTime);
        
        result.Value.ShouldBe(Result.Success);
        secondBatch.Status.ShouldBe(BatchStatus.Washing);
        secondBatch.LastWashingActivityAt.ShouldBe(pastTime);
        secondBatch.WashingStartedAt.ShouldBe(pastTime);
        lineQueue.Batches.ElementAt(0).ShouldBe(secondBatch);
        lineQueue.Batches.Count.ShouldBe(3);
        lineQueue.Batches.ShouldNotContain(firstBatch);
    }
    
    [Fact]
    public void StartWashingBatch_ShouldFail_WhenNotFirstBatchInQueue()
    {
        var lineQueue = new LineQueue("LINE123", true, []);
        var firstBatch = CreateNewBatch();
        lineQueue.AddBatch(firstBatch);
        var secondBatch = CreateNewBatch();
        lineQueue.AddBatch(secondBatch);
        lineQueue.AddBatch(CreateNewBatch());
        lineQueue.AddBatch(CreateNewBatch());
        
        var result = lineQueue.StartBatchWashing(secondBatch.Id, DateTimeOffset.Now - TimeSpan.FromMinutes(10));
        
        result.IsError.ShouldBeTrue();
        result.FirstError.Code.ShouldBe(LineQueueErrors.ValidationCannotStartWashingBatchBecauseItIsNotNextInQueue.Code);
    }
    
    [Fact]
    public void StartWashingBatch_ShouldFail_WhenNotFirstBatchInWashingAndThirdIsGonnaBeActivated()
    {
        var lineQueue = new LineQueue("LINE123", true, []);
        var firstBatch = CreateNewBatch();
        lineQueue.AddBatch(firstBatch);
        firstBatch.StartWashing(DateTimeOffset.Now);
        lineQueue.AddBatch(CreateNewBatch());
        var thirdBatch = CreateNewBatch();
        lineQueue.AddBatch(thirdBatch);
        lineQueue.AddBatch(CreateNewBatch());
        
        var result = lineQueue.StartBatchWashing(thirdBatch.Id, DateTimeOffset.Now - TimeSpan.FromMinutes(10));
        
        result.IsError.ShouldBeTrue();
        result.FirstError.Code.ShouldBe(LineQueueErrors.ValidationCannotStartWashingBatchBecauseItIsNotNextInQueue.Code);
    }
    
    private static Batch CreateNewBatch() 
        => new(null, "DepositorCode", "KitCode", "KitNumber", 10, "PackagingCode", 20, "DefiningPackagingCode", "KitSapDefinitionCode",  TimeSpan.FromMinutes(5));
}