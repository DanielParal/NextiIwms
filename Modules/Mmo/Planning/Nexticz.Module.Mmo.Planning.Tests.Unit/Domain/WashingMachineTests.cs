using ErrorOr;
using Nexticz.Module.Mmo.Planning.Domain.BatchEntity;
using Nexticz.Module.Mmo.Planning.Domain.LineQueueEntity;
using Nexticz.Module.Mmo.Planning.Domain.WashingMachineAggregate;
using Shouldly;

namespace Nexticz.Module.Mmo.Planning.Tests.Unit.Domain;

public class WashingMachineTests
{
    [Fact]
    public void Constructor_ShouldInitializeWashingMachine_WhenValidArguments()
    {
        const string washingMachineCode = "WashingMachine";
        
        var washingMachine = CreateNewWashingMachine(["Line1", "Line2"], code: washingMachineCode);
        
        washingMachine.Code.ShouldBe(washingMachineCode);
        washingMachine.IsOneLineMachine.ShouldBeFalse();
        washingMachine.LineQueues.Count.ShouldBe(2);
    }

    [Fact]
    public void Constructor_ShouldThrowException_WhenInvalidWashingMachineCode()
    {
        Should.Throw<ArgumentException>(() => CreateNewWashingMachine(["Line1", "Line2"], string.Empty));
    }
    
    [Fact]
    public void Constructor_ShouldThrowException_WhenNoLines()
    {
        Should.Throw<ArgumentException>(() => new WashingMachine("Code", WashingMachineStatus.Working, []));
    }
    
    [Fact]
    public void Constructor_ShouldThrowException_WhenMoreLines()
    {
        Should.Throw<ArgumentException>(() => CreateNewWashingMachine(["Line1", "Line2", "Line3"]));
    }
    
    [Fact]
    public void Constructor_ShouldThrowException_WhenNotDistinctLineCodes()
    {
        Should.Throw<ArgumentException>(() => CreateNewWashingMachine(["Line1", "Line1"]));
    }
    
    [Fact]
    public void AddBatch_ShouldAddBatch_WhenValid()
    {
        // Arrange
        const string lineCode1 = "Line1";
        const string lineCode2 = "Line2";
        var washingMachine = CreateNewWashingMachine([lineCode1, lineCode2]);
        var batch = CreateNewBatch();
        var batch2 = CreateNewBatch();

        // Act
        var result = washingMachine.AddBatch(batch, lineCode1);
        var result2 = washingMachine.AddBatch(batch2, lineCode2);

        // Assert
        result.Value.ShouldBe(Result.Success);
        washingMachine.LineQueues[0].Batches.ShouldContain(batch);
        
        result2.Value.ShouldBe(Result.Success);
        washingMachine.LineQueues[1].Batches.ShouldContain(batch2);
    }
    
    [Fact]
    public void AddSimultaneousBatch_ShouldAddBatchToEachLine_WhenValid()
    {
        // Arrange
        const string lineCode1 = "Line1";
        const string lineCode2 = "Line2";
        var washingMachine = CreateNewWashingMachine([lineCode1, lineCode2]);
        washingMachine.AddBatch(CreateNewBatch(), lineCode1);
        washingMachine.AddBatch(CreateNewBatch(), lineCode1);
        washingMachine.AddBatch(CreateNewBatch(), lineCode2);
        
        var sisterBatchId1 = Guid.NewGuid();
        var sisterBatchId2 = Guid.NewGuid();
        var sisterBatch1 = CreateNewBatch(sisterBatchId1, sisterBatchId2);
        var sisterBatch2 = CreateNewBatch(sisterBatchId2, sisterBatchId1);

        // Act
        washingMachine.AddSisterBatches(sisterBatch1, sisterBatch2);

        // Assert
        washingMachine.LineQueues[0].Batches.ShouldContain(sisterBatch1);
        washingMachine.LineQueues[0].Batches.Count.ShouldBe(3);
        washingMachine.LineQueues[1].Batches.ShouldContain(sisterBatch2);
        washingMachine.LineQueues[1].Batches.Count.ShouldBe(2);
    }
    
    [Fact]
    public void AddSimultaneousBatch_ShouldFail_WhenAddingSisterBatchesToOneLineWashingMachine()
    {
        // Arrange
        const string lineCode1 = "Line1";
        var washingMachine = CreateNewWashingMachine([lineCode1]);
        washingMachine.AddBatch(CreateNewBatch(), lineCode1);
        washingMachine.AddBatch(CreateNewBatch(), lineCode1);
        washingMachine.AddBatch(CreateNewBatch(), lineCode1);
        
        var sisterBatchId1 = Guid.NewGuid();
        var sisterBatchId2 = Guid.NewGuid();
        var sisterBatch1 = CreateNewBatch(sisterBatchId1, sisterBatchId2);
        var sisterBatch2 = CreateNewBatch(sisterBatchId2, sisterBatchId1);

        // Act
        var result = washingMachine.AddSisterBatches(sisterBatch1, sisterBatch2);

        // Assert
        result.IsError.ShouldBeTrue();
        result.FirstError.Code.ShouldBe(WashingMachineErrors.ValidationCannotScheduleSimultaneousBatchOnOneLineWashingMachine.Code);
    }
    
    [Fact]
    public void RemoveBatch_ShouldRemoveSingleBatch_WhenRemovingSingleBatch()
    {
        // Arrange
        const string lineCode1 = "Line1";
        const string lineCode2 = "Line2";
        var washingMachine = CreateNewWashingMachine([lineCode1, lineCode2]);
        washingMachine.AddBatch(CreateNewBatch(), lineCode1);
        var batchToBeDeleted = CreateNewBatch();
        washingMachine.AddBatch(batchToBeDeleted, lineCode1);
        washingMachine.AddBatch(CreateNewBatch(), lineCode2);
        
        var sisterBatchId1 = Guid.NewGuid();
        var sisterBatchId2 = Guid.NewGuid();
        var sisterBatch1 = CreateNewBatch(sisterBatchId1, sisterBatchId2);
        var sisterBatch2 = CreateNewBatch(sisterBatchId2, sisterBatchId1);
        washingMachine.AddSisterBatches(sisterBatch1, sisterBatch2);
        
        washingMachine.AddBatch(CreateNewBatch(), lineCode1);
        washingMachine.AddBatch(CreateNewBatch(), lineCode2);

        var countBeforeDelete = washingMachine.LineQueues[0].Batches.Count;
        
        // Act
        washingMachine.RemoveBatch(batchToBeDeleted.Id);

        // Assert
        countBeforeDelete.ShouldBe(4);
        washingMachine.LineQueues[0].Batches.Count.ShouldBe(3);
        washingMachine.LineQueues[0].Batches.ShouldNotContain(batchToBeDeleted);
        washingMachine.LineQueues[1].Batches.Count.ShouldBe(3);
    }
    
    [Fact]
    public void RemoveBatch_ShouldRemoveSisterBatches_WhenRemovingSisterBatches()
    {
        // Arrange
        const string lineCode1 = "Line1";
        const string lineCode2 = "Line2";
        var washingMachine = CreateNewWashingMachine([lineCode1, lineCode2]);
        washingMachine.AddBatch(CreateNewBatch(), lineCode1);
        washingMachine.AddBatch(CreateNewBatch(), lineCode1);
        washingMachine.AddBatch(CreateNewBatch(), lineCode2);
        
        var sisterBatchId1 = Guid.NewGuid();
        var sisterBatchId2 = Guid.NewGuid();
        var sisterBatch1 = CreateNewBatch(sisterBatchId1, sisterBatchId2);
        var sisterBatch2 = CreateNewBatch(sisterBatchId2, sisterBatchId1);
        washingMachine.AddSisterBatches(sisterBatch1, sisterBatch2);
        
        washingMachine.AddBatch(CreateNewBatch(), lineCode1);
        washingMachine.AddBatch(CreateNewBatch(), lineCode2);

        var line1LengthBeforeDelete = washingMachine.LineQueues[0].Batches.Count;
        var line2LengthBeforeDelete = washingMachine.LineQueues[1].Batches.Count;
        
        // Act
        washingMachine.RemoveBatch(sisterBatch1.Id);

        // Assert
        line1LengthBeforeDelete.ShouldBe(4);
        washingMachine.LineQueues[0].Batches.Count.ShouldBe(3);
        washingMachine.LineQueues[0].Batches.ShouldNotContain(sisterBatch1);
        
        line2LengthBeforeDelete.ShouldBe(3);
        washingMachine.LineQueues[1].Batches.Count.ShouldBe(2);
        washingMachine.LineQueues[1].Batches.ShouldNotContain(sisterBatch2);
    }
    
    [Fact]
    public void RemoveBatch_ShouldFail_WhenBatchDoesNotExist()
    {
        // Arrange
        const string lineCode1 = "Line1";
        const string lineCode2 = "Line2";
        var washingMachine = CreateNewWashingMachine([lineCode1, lineCode2]);
        washingMachine.AddBatch(CreateNewBatch(), lineCode1);
        washingMachine.AddBatch(CreateNewBatch(), lineCode1);
        washingMachine.AddBatch(CreateNewBatch(), lineCode2);
        
        // Act
        var result = washingMachine.RemoveBatch(Guid.NewGuid());

        // Assert
        result.IsError.ShouldBeTrue();
        result.FirstError.Code.ShouldBe(WashingMachineErrors.ValidationBatchIdNotFoundInQueue.Code);
    }

    [Fact]
    public void UpdateBatchKitsCount_ShouldUpdateSingleBatchKitsCount_WhenSmallerNumber()
    {
        // Arrange
        const string lineCode1 = "Line1";
        const string lineCode2 = "Line2";
        var batchToBeUpdated = CreateNewBatch(kitsCount: 15);
        var washingMachine = CreateNewWashingMachine([lineCode1, lineCode2]);
        washingMachine.AddBatch(CreateNewBatch(), lineCode1);
        washingMachine.AddBatch(batchToBeUpdated, lineCode1);
        washingMachine.AddBatch(CreateNewBatch(), lineCode2);
        
        // Act
        var result = washingMachine.UpdateBatchKitsCount(batchToBeUpdated.Id, 10);

        // Assert
        result.IsError.ShouldBeFalse();
        batchToBeUpdated.KitsCount.ShouldBe(10);
    }
    
    [Fact]
    public void UpdateBatchKitsCount_ShouldUpdateSisterBatchesKitsCount_WhenDifferentNumber()
    {
        // Arrange
        const string lineCode1 = "Line1";
        const string lineCode2 = "Line2";
        var batchId = Guid.NewGuid();
        var sisterBatchId = Guid.NewGuid();
        var batchToBeUpdated = CreateNewBatch(id: batchId, sisterBatchId: sisterBatchId, kitsCount: 15);
        var sisterBatchToBeUpdated = CreateNewBatch(id: sisterBatchId, sisterBatchId: batchId, kitsCount: 6);
        var washingMachine = CreateNewWashingMachine([lineCode1, lineCode2]);
        washingMachine.AddBatch(CreateNewBatch(), lineCode1);
        washingMachine.AddSisterBatches(batchToBeUpdated, sisterBatchToBeUpdated);
        
        // Act
        var result = washingMachine.UpdateBatchKitsCount(batchToBeUpdated.Id, 8);

        // Assert
        result.Value.ShouldBe(Result.Success);
        batchToBeUpdated.KitsCount.ShouldBe(8);
        sisterBatchToBeUpdated.KitsCount.ShouldBe(8);
    }
    
    [Fact]
    public void UpdateBatchKitsCount_ShouldIncreaseSisterBatchesKitsCount_WhenPositiveValue()
    {
        // Arrange
        const string lineCode1 = "Line1";
        const string lineCode2 = "Line2";
        var batchId = Guid.NewGuid();
        var sisterBatchId = Guid.NewGuid();
        var batchToBeUpdated = CreateNewBatch(id: batchId, sisterBatchId: sisterBatchId, kitsCount: 15);
        var sisterBatchToBeUpdated = CreateNewBatch(id: sisterBatchId, sisterBatchId: batchId, kitsCount: 6);
        var washingMachine = CreateNewWashingMachine([lineCode1, lineCode2]);
        washingMachine.AddBatch(CreateNewBatch(), lineCode1);
        washingMachine.AddSisterBatches(batchToBeUpdated, sisterBatchToBeUpdated);
        
        // Act
        var result = washingMachine.UpdateBatchKitsCount(batchToBeUpdated.Id, 25);

        // Assert
        result.Value.ShouldBe(Result.Success);
        batchToBeUpdated.KitsCount.ShouldBe(25);
        sisterBatchToBeUpdated.KitsCount.ShouldBe(25);
    }
    
    [Fact]
    public void UpdateBatchKitsCount_ShouldFail_WhenUpdateToZero()
    {
        // Arrange
        const string lineCode1 = "Line1";
        const string lineCode2 = "Line2";
        var batchToBeUpdated = CreateNewBatch(kitsCount: 5);
        var washingMachine = CreateNewWashingMachine([lineCode1, lineCode2]);
        washingMachine.AddBatch(CreateNewBatch(), lineCode1);
        washingMachine.AddBatch(batchToBeUpdated, lineCode1);
        washingMachine.AddBatch(CreateNewBatch(), lineCode2);
        
        // Act
        var result = washingMachine.UpdateBatchKitsCount(batchToBeUpdated.Id, 0);

        // Assert
        result.IsError.ShouldBeTrue();
        result.FirstError.Code.ShouldBe(BatchErrors.ValidationKitsCountHasToBeGreaterThan0.Code);
        batchToBeUpdated.KitsCount.ShouldBe(5);
    }
    
    [Fact]
    public void MoveBatch_ShouldMoveBatch_WhenValid()
    {
        // Arrange
        const string lineCode1 = "Line1";
        const string lineCode2 = "Line2";
        var batchToBeUpdated = CreateNewBatch();
        var washingMachine = CreateNewWashingMachine([lineCode1, lineCode2]);
        washingMachine.AddBatch(batchToBeUpdated, lineCode1);
        washingMachine.AddBatch(CreateNewBatch(), lineCode1);
        washingMachine.AddBatch(CreateNewBatch(), lineCode1);
        washingMachine.AddBatch(CreateNewBatch(), lineCode1);
        washingMachine.AddBatch(CreateNewBatch(), lineCode2);
        
        // Act
        var result = washingMachine.MoveBatch(batchToBeUpdated.Id, 2);

        // Assert
        result.IsError.ShouldBeFalse();
        washingMachine.LineQueues[0].Batches.ElementAt(2).ShouldBe(batchToBeUpdated);
        washingMachine.LineQueues[0].Batches.Count.ShouldBe(4);
    }

    private static WashingMachine CreateNewWashingMachine(string[] lineCodes, string code = "Code") => new(code, WashingMachineStatus.Working, lineCodes.Select(lc => new LineQueue(lc, true, [])).ToArray());
    private static Batch CreateNewBatch(Guid? id = null, Guid? sisterBatchId = null, int kitsCount = 10)
        => new(sisterBatchId, "DepositorCode", "KitCode", "KitNumber", kitsCount, "PackagingCode", 20, "DefiningPackagingCode", "KitSapDefinitionCode", TimeSpan.FromMinutes(5), null, null, id);
}