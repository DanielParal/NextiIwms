using Nexticz.Module.Mmo.SharedKernel;
using Nexticz.Module.Mmo.SharedKernel.DomainCore;
using Nexticz.Module.Mmo.SharedKernel.Efficiencies;
using Nexticz.Module.Mmo.Washing.Domain.WashingMachineSpeedAggregate;

namespace Nexticz.Module.Mmo.Washing.Domain.KitWashCycleEntity;

public class KitWashCycle : Entity
{
    public Guid? SisterKitId { get; private set; }
    public Guid BatchId { get; private set; }
    public DateTimeOffset StartDate { get; private set; }
    public DateTimeOffset EndDate { get; private set; }
    public TimeSpan OptimalDuration { get; private set; }
    public string WorkerName { get; private set; }
    public int OrderId { get; private set; }
    public int GlobalKitsCount { get; private set; }
    public double Efficiency { get; private set; }
    public int WashingMachineSpeed { get; private set; }
    public SpeedLevel WashingMachineSpeedLevel { get; private set; }
    
    // We need private constructor due to Marten deserialization
    private KitWashCycle() {}

    public KitWashCycle(
        Guid? sisterKitId,
        Guid batchId,
        DateTimeOffset startDate,
        DateTimeOffset endDate,
        TimeSpan optimalDuration,
        string workerName,
        int orderId,
        int globalKitsCount,
        int washingMachineSpeed,
        SpeedLevel washingMachineSpeedLevel,
        Guid? id = null)
        : base(id ?? Guid.NewGuid())
    {
        SisterKitId = sisterKitId;
        BatchId = batchId;
        StartDate = startDate;
        EndDate = endDate;
        OptimalDuration = optimalDuration;
        WorkerName = workerName;
        OrderId = orderId;
        GlobalKitsCount = globalKitsCount;
        WashingMachineSpeed = washingMachineSpeed;
        WashingMachineSpeedLevel = washingMachineSpeedLevel;
        Efficiency = EfficiencyCalculator.Calculate(startDate, endDate, optimalDuration);
    }
}