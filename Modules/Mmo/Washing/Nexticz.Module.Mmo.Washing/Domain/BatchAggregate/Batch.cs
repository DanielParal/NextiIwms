using ErrorOr;
using Nexticz.Module.Mmo.Planning.Contracts.WashingMachines;
using Nexticz.Module.Mmo.SharedKernel;
using Nexticz.Module.Mmo.SharedKernel.DomainCore;
using Nexticz.Module.Mmo.Washing.Domain.BatchAggregate.Events;
using Nexticz.Module.Mmo.Washing.Domain.KitWashCycleEntity;
using Nexticz.Module.Mmo.Washing.Domain.PrintingEntity;
using Nexticz.Module.Mmo.Washing.Domain.SpecialInformationEntity;
using Nexticz.Module.Mmo.Washing.Domain.WashingMachineSpeedAggregate;

namespace Nexticz.Module.Mmo.Washing.Domain.BatchAggregate;

public class Batch : AggregateRoot
{
    public Guid? SisterBatchId { get; private set; }
    public string WashingMachineCode { get; private set; }
    public int WashingMachineLength { get; private set; }
    public string LineCode { get; private set; }
    public string KitCode { get; private set; }
    public string KitNumber { get; private set; }
    public int PlannedKitsCount { get; private set; }
    public string KitSapDefinitionCode { get; private set; }
    public string KitSapDefinitionName { get; private set; }
    public string SapBarcode { get; private set; }
    public string PackagingCode { get; private set; }
    public int OptimalPackagingSpeedOnCurrentMachine { get; private set; }
    public SpeedLevel OptimalPackagingSpeedOnCurrentMachineLevel { get; private set; }
    public int PackagingSpeed { get; private set; }
    public SpeedLevel PackagingSpeedLevel { get; private set; }
    public decimal PackagingHeight { get; private set; }
    public string DefiningPackagingCode { get; private set; }
    public TimeSpan OptimalKitDuration { get; private set; }
    public DateTimeOffset DateActivated { get; private set; }
    public DateTimeOffset? PreviousBatchLastKitEndDate { get; private set; }
    public bool ShouldFirstKitStartAfterPreviousBatchLastKitEndDate { get; private set; }
    public SpecialInformation? SpecialInformation { get; private set; }
    private readonly List<KitWashCycle> _kitWashCycles = [];
    private readonly List<Printing> _printings = [];
    public IReadOnlyCollection<KitWashCycle> KitWashCycles => _kitWashCycles.AsReadOnly();
    public IReadOnlyCollection<Printing> Printings => _printings.AsReadOnly();
    
    // We need private constructor due to Marten deserialization
    private Batch() {}

    public Batch(
        Guid? sisterBatchId,
        string washingMachineCode,
        int washingMachineLength,
        string lineCode,
        string kitCode,
        string kitNumber,
        int plannedKitsCount,
        string kitSapDefinitionCode,
        string kitSapDefinitionName,
        string sapBarcode,
        string packagingCode,
        int optimalPackagingSpeedOnCurrentMachine,
        SpeedLevel optimalPackagingSpeedOnCurrentMachineLevel,
        int packagingSpeed,
        SpeedLevel packagingSpeedLevel,
        decimal packagingHeight,
        string definingPackagingCode,
        TimeSpan optimalKitDuration,
        DateTimeOffset dateActivated,
        DateTimeOffset? previousBatchLastKitEndDate,
        bool shouldFirstKitStartAfterPreviousBatchLastKitEndDate,
        KitWashCycle[] kitWashCycles,
        Printing[] printings,
        SpecialInformation? specialInformation,
        Guid? id = null)
        : base(id ?? Guid.NewGuid())
    {
        SisterBatchId = sisterBatchId;
        WashingMachineCode = washingMachineCode;
        WashingMachineLength = washingMachineLength;
        LineCode = lineCode;
        KitCode = kitCode;
        KitNumber = kitNumber;
        PlannedKitsCount = plannedKitsCount;
        KitSapDefinitionCode = kitSapDefinitionCode;
        KitSapDefinitionName = kitSapDefinitionName;
        SapBarcode = sapBarcode;
        OptimalKitDuration = optimalKitDuration;
        PackagingCode = packagingCode;
        OptimalPackagingSpeedOnCurrentMachine = optimalPackagingSpeedOnCurrentMachine;
        OptimalPackagingSpeedOnCurrentMachineLevel = optimalPackagingSpeedOnCurrentMachineLevel;
        PackagingSpeed = packagingSpeed;
        PackagingSpeedLevel = packagingSpeedLevel;
        PackagingHeight = packagingHeight;
        DefiningPackagingCode = definingPackagingCode;
        DateActivated = dateActivated;
        PreviousBatchLastKitEndDate = previousBatchLastKitEndDate;
        ShouldFirstKitStartAfterPreviousBatchLastKitEndDate = shouldFirstKitStartAfterPreviousBatchLastKitEndDate;
        _printings = printings.ToList();
        _kitWashCycles = kitWashCycles.ToList();
        SpecialInformation = specialInformation;
    }

    public ErrorOr<Success> CanFinishKit(string workerName)
    {
        if (SisterBatchId is null && IsSpecialInformationConfirmationNeededByWorker(workerName))
            return BatchErrors.ValidationBatchIsNotConfirmedByWorker;

        return Result.Success;
    }
    
    public void DetachSisterBatch()
    {
        SisterBatchId = null;
    }

    public bool IsSpecialInformationConfirmationNeededByWorker(string workerName)
    {
        if (SpecialInformation is null)
            return false;
        
        return SpecialInformation.Confirmations.All(x =>
            !x.WorkerName.Equals(workerName, StringComparison.InvariantCultureIgnoreCase));
    }

    public DateTimeOffset GetNextKitWashCycleStartDate(DateTimeOffset dateFinished)
    {
        if (KitWashCycles.Count == 0)
        {
            return GetFirstKitStartDate(dateFinished);
        }
        
        var lastCycle = KitWashCycles.Last();
        return lastCycle.EndDate;
    }
    
    public int GetNextOrderId()
    {
        if (KitWashCycles.Count == 0)
        {
            return 1;
        }
        
        return KitWashCycles.Count + 1;
    }

    private DateTimeOffset GetFirstKitStartDate(DateTimeOffset dateFinished)
    {
        if (ShouldFirstKitStartAfterPreviousBatchLastKitEndDate && PreviousBatchLastKitEndDate is not null)
            return PreviousBatchLastKitEndDate.Value;
        
        return dateFinished - OptimalKitDuration;
    }

    public void ChangePlannedKitsCount(int plannedKitsCount)
    {
        if (plannedKitsCount < 0)
            throw new ArgumentException("Planned kits count cannot be negative.", nameof(plannedKitsCount));
        
        PlannedKitsCount = plannedKitsCount;
    }

    public void Apply(BatchCreatedEvent @event)
    {
        Id = @event.Id;
        SisterBatchId = @event.SisterBatchId;
        WashingMachineCode = @event.WashingMachineCode;
        WashingMachineLength = @event.WashingMachineLength;
        LineCode = @event.LineCode;
        KitCode = @event.KitCode;
        KitNumber = @event.KitNumber;
        PlannedKitsCount = @event.PlannedKitsCount;
        KitSapDefinitionCode = @event.KitSapDefinitionCode;
        KitSapDefinitionName = @event.KitSapDefinitionName;
        SapBarcode = @event.SapBarcode;
        OptimalKitDuration = @event.OptimalKitDuration;
        PackagingCode = @event.PackagingCode;
        OptimalPackagingSpeedOnCurrentMachine = @event.OptimalPackagingSpeedOnCurrentMachine;
        OptimalPackagingSpeedOnCurrentMachineLevel = @event.OptimalPackagingSpeedOnCurrentMachineLevel;
        PackagingSpeed = @event.PackagingSpeed;
        PackagingSpeedLevel = @event.PackagingSpeedLevel;
        PackagingHeight = @event.PackagingHeight;
        DefiningPackagingCode = @event.DefiningPackagingCode;
        DateActivated = @event.DateActivated;
        PreviousBatchLastKitEndDate = @event.PreviousBatchLastKitEndDate;
        ShouldFirstKitStartAfterPreviousBatchLastKitEndDate = @event.ShouldFirstKitStartAfterPreviousBatchLastKitEndDate;
        SpecialInformation = @event.SpecialInformation;
    }
    
    public void Apply(KitWashCycleFinishedEvent @event)
    {
        var kitWashCycle = new KitWashCycle(@event.SisterKitId, @event.BatchId, @event.StartDate, @event.FinishedDate, 
            @event.OptimalDuration, @event.WorkerName, @event.OrderId, @event.GlobalKitsCount, 
            @event.WashingMachineSpeed, @event.WashingMachineSpeedLevel, @event.Id);

        _kitWashCycles.Add(kitWashCycle);
    }
    
    public void Apply(SisterBatchDetachedEvent @event)
    {
        DetachSisterBatch();
    }
    
    public void Apply(PrintingCreatedEvent @event)
    {
        var printing = new Printing(@event.BatchId, @event.KitId, @event.DatePrinted, @event.Status, @event.Type, @event.FailureReason, @event.Id);
        _printings.Add(printing);
    }
    
    public void Apply(SpecialInformationConfirmedEvent @event)
    {
        SpecialInformation?.AddConfirmation(@event.WorkerName, @event.DateConfirmed);
    }
    
    public void Apply(BatchPlannedKitsCountChanged @event)
    {
        ChangePlannedKitsCount(@event.PlannedKitsCount);;
    }
}