using Nexticz.Module.Mmo.Planning.Domain.BatchEntity;
using Shouldly;

namespace Nexticz.Module.Mmo.Planning.Tests.Unit.Domain;

public class BatchTests
{
    [Fact]
    public void BatchConstructor_ShouldThrowException_WhenKitCodeIsNullOrEmpty()
    {
        var exception = Should.Throw<ArgumentException>(() =>
            new Batch(null, "DepositorCode", null, "KitNumber", 1, "PackagingCode", 20,"DefiningPackagingCode", "KitSapDefinitionCode", TimeSpan.FromMinutes(1)));
        
        exception.ParamName.ShouldBe("kitCode");
    }
    
    [Fact]
    public void BatchConstructor_ShouldThrowException_WhenKitNumberIsNullOrEmpty()
    {
        var exception = Should.Throw<ArgumentException>(() =>
            new Batch(null, "DepositorCode", "KitCode", null, 1, null, 20, "DefiningPackagingCode", "KitSapDefinitionCode",  TimeSpan.FromMinutes(1)));
        
        exception.ParamName.ShouldBe("kitNumber");
    }

    [Fact]
    public void BatchConstructor_ShouldThrowException_WhenPackagingCodeIsNullOrEmpty()
    {
        var exception = Should.Throw<ArgumentException>(() =>
            new Batch(null, "DepositorCode", "KitCode", "KitNumber", 1, null, 20, "DefiningPackagingCode", "KitSapDefinitionCode", TimeSpan.FromMinutes(1)));
        
        exception.ParamName.ShouldBe("packagingCode");
    }

    [Fact]
    public void BatchConstructor_ShouldThrowException_WhenKitsCountIsZeroOrNegative()
    {
        var exception = Should.Throw<ArgumentOutOfRangeException>(() =>
            new Batch(null, "DepositorCode", "KitCode", "KitNumber", 0, "PackagingCode", 20, "DefiningPackagingCode", "KitSapDefinitionCode", TimeSpan.FromMinutes(1)));
        
        exception.ParamName.ShouldBe("kitsCount");
    }

    [Fact]
    public void BatchConstructor_ShouldThrowException_WhenOptimalKitDurationIsZeroOrNegative()
    {
        var exception = Should.Throw<ArgumentOutOfRangeException>(() =>
            new Batch(null, "DepositorCode", "KitCode", "KitNumber", 1, "PackagingCode", 20, "DefiningPackagingCode", "KitSapDefinitionCode", TimeSpan.Zero));
        
        exception.ParamName.ShouldBe("optimalKitDuration");
    }
    
    [Fact]
    public void BatchConstructor_ShouldReturnCorrectCalculatedValues_WhenValidInputData()
    {
        var batch = new Batch(Guid.NewGuid(), "DepositorCode", "KitCode", "KitNumber", 10, "PackagingCode", 20, "DefiningPackagingCode", "KitSapDefinitionCode",  TimeSpan.FromMinutes(5));
        
        batch.OptimalBatchDuration.ShouldBe(TimeSpan.FromMinutes(50));
        batch.KitsLeft.ShouldBe(10);
        batch.HasSisterBatch.ShouldBeTrue();
    }

    [Fact]
    public void CanUpdateKitsCount_ShouldSucceed_WhenKitsCountToChangeIsTheSameValueLikeKitsCount()
    {
        var batch = new Batch(null, "DepositorCode", "KitCode", "KitNumber", 1, "PackagingCode", 20, "DefiningPackagingCode", "KitSapDefinitionCode",  TimeSpan.FromMinutes(1));
        
        var result = batch.CanUpdateKitsCount(1);
        
        result.IsError.ShouldBeFalse();
        batch.KitsCount.ShouldBe(1);
    }
    
    [Fact]
    public void CanUpdateKitsCount_ShouldReturnError_WhenKitsToDecreaseIsEqualKitsLeft()
    {
        var batch = new Batch(null, "DepositorCode", "KitCode", "KitNumber", 2, "PackagingCode", 20, "DefiningPackagingCode", "KitSapDefinitionCode",  TimeSpan.FromMinutes(1));
        
        var result = batch.CanUpdateKitsCount(-2);
        
        result.IsError.ShouldBeTrue();
        result.FirstError.Code.ShouldBe(BatchErrors.ValidationNotEnoughKitsLeftForDecrease(batch.KitsLeft).Code);
    }

    [Fact]
    public void UpdateKitsCount_ShouldReduceKitsCount_WhenValidCountIsGiven()
    {
        var batch = new Batch(null, "DepositorCode", "KitCode", "KitNumber", 3, "PackagingCode", 20, "DefiningPackagingCode", "KitSapDefinitionCode",TimeSpan.FromMinutes(1));
        
        var result = batch.UpdateKitsCount(4);
        
        result.IsError.ShouldBeFalse();
        batch.KitsCount.ShouldBe(4);
    }
    
    [Fact]
    public void FinishKit_ShouldDecreaseKitsLeft_WhenKitIsFinished()
    {
        var batch = new Batch(null, "DepositorCode", "KitCode", "KitNumber", 5, "PackagingCode", 20, "DefiningPackagingCode", "KitSapDefinitionCode", TimeSpan.FromMinutes(1));
        
        batch.FinishKit(DateTimeOffset.Now);
        
        batch.KitsLeft.ShouldBe(4);
        batch.LastWashingActivityAt.ShouldNotBeNull();
    }
}