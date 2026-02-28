using Nexticz.Module.Mmo.Reporting.Domain.SpecialInformationEntity;
using Nexticz.Module.Mmo.Reporting.Domain.WashingMachineSpeedAggregate;
using Nexticz.Module.Mmo.SharedKernel;
using Nexticz.Module.Mmo.SharedKernel.DomainCore;
using Nexticz.Module.Mmo.SharedKernel.Efficiencies;

namespace Nexticz.Module.Mmo.Reporting.Domain.KitAggregate;

public class Kit : AggregateRoot
{
    public Guid KitIdFromWashing { get; private set; }
    public Guid? SisterKitIdFromWashing { get; private set; }
    public Guid ShiftId { get; private set; }
    public string WashingMachineCode { get; private set; }
    public string LineCode { get; private set; }
    public int OrderId { get; private set; }
    public int TotalPlannedKitsCountInBatch { get; private set; }
    public Guid BatchId { get; private set; }
    public Guid? SisterBatchId { get; private set; }
    public int WashingMachineSpeed { get; private set; }
    public SpeedLevel WashingMachineSpeedLevel { get; private set; }
    public string KitCode { get; private set; }
    public string KitNumber { get; private set; }
    public string KitSapDefinitionCode { get; private set; }
    public string KitSapDefinitionName { get; private set; }
    public string? SapBarcode { get; private set; }
    public string PackagingCode { get; private set; }
    public int OptimalPackagingSpeedOnWashingMachine { get; private set; }
    public SpeedLevel OptimalPackagingSpeedOnWashingMachineLevel { get; private set; }
    public string DefiningPackagingCode { get; private set; }
    public TimeSpan RealTimeKitDuration { get; private set; }
    public TimeSpan OptimalKitDuration { get; private set; }
    public DateTimeOffset WashingStarted { get; private set; }
    public DateTimeOffset WashingEnded { get; private set; }
    public string DeclaredBy { get; private set; }
    public int GlobalKitsCount { get; private set; }
    public double Efficiency => EfficiencyCalculator.Calculate(RealTimeKitDuration, OptimalKitDuration);
    public DateTimeOffset CreatedAt { get; private set; }
    public string? UpdatedBy { get; private set; }
    public SpecialInformation? SpecialInformation { get; private set; }
    
    // We need private constructor due to Marten deserialization
    private Kit() {}
    
    public Kit(
        Guid kitIdFromWashing,
        Guid? sisterKitIdFromWashing,
        Guid shiftId,
        Guid batchId,
        Guid? sisterBatchId,
        int orderId,
        int totalPlannedKitsCountInBatch,
        string washingMachineCode,
        string lineCode,
        int washingMachineSpeed,
        SpeedLevel washingMachineSpeedLevel,
        string kitCode,
        string kitNumber,
        string? sapBarcode,
        string kitSapDefinitionCode,
        string kitSapDefinitionName,
        string packagingCode,
        int optimalPackagingSpeedOnWashingMachine,
        SpeedLevel optimalPackagingSpeedOnWashingMachineLevel,
        string definingPackagingCode,
        TimeSpan realTimeKitDuration,
        TimeSpan optimalKitDuration,
        DateTimeOffset washingStarted,
        DateTimeOffset washingEnded,
        string declaredBy,
        int globalKitsCount,
        DateTimeOffset createdAt,
        string? updatedBy,
        SpecialInformation? specialInformation,
        Guid? id = null)
        : base(id ?? Guid.NewGuid())
    {
        KitIdFromWashing = kitIdFromWashing;
        SisterKitIdFromWashing = sisterKitIdFromWashing;
        ShiftId = shiftId;
        BatchId = batchId;
        SisterBatchId = sisterBatchId;
        OrderId = orderId;
        TotalPlannedKitsCountInBatch = totalPlannedKitsCountInBatch;
        WashingMachineCode = washingMachineCode;
        LineCode = lineCode;
        WashingMachineSpeed = washingMachineSpeed;
        WashingMachineSpeedLevel = washingMachineSpeedLevel;
        KitCode = kitCode;
        KitNumber = kitNumber;
        SapBarcode = sapBarcode;
        KitSapDefinitionCode = kitSapDefinitionCode;
        KitSapDefinitionName = kitSapDefinitionName;
        PackagingCode = packagingCode;
        OptimalPackagingSpeedOnWashingMachine = optimalPackagingSpeedOnWashingMachine;
        OptimalPackagingSpeedOnWashingMachineLevel = optimalPackagingSpeedOnWashingMachineLevel;       
        DefiningPackagingCode = definingPackagingCode;
        RealTimeKitDuration = realTimeKitDuration;
        OptimalKitDuration = optimalKitDuration;
        WashingStarted = washingStarted;
        WashingEnded = washingEnded;
        DeclaredBy = declaredBy;
        GlobalKitsCount = globalKitsCount;
        CreatedAt = createdAt;
        UpdatedBy = updatedBy;
        SpecialInformation = specialInformation;
    }
}