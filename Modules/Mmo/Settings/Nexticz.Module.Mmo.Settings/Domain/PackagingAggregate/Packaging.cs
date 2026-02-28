using Nexticz.Module.Mmo.Settings.Domain.PackagingAggregate.Events;
using Nexticz.Module.Mmo.SharedKernel;
using Nexticz.Module.Mmo.SharedKernel.DomainCore;

namespace Nexticz.Module.Mmo.Settings.Domain.PackagingAggregate;

public class Packaging : AggregateRoot
{
    public string Code { get; private set; }
    public string PackagingTypeCode { get; private set; }
    public string DepositorCode { get; private set; }
    public string PackagingCirculationCode { get; private set; }
    public string CustomerNumber { get; private set; }
    public string Name { get; private set; }
    public bool MustBeWashed { get; private set; }
    public Dimensions Dimensions { get; private set; } 
    public decimal Weight { get; private set; }
    public WashingMachineSpeed[] WashingMachineSpeeds { get; private set; }

    // We need private constructor due to Marten deserialization
    private Packaging() {}
    
    public Packaging(
        string code,
        string packagingTypeCode,
        string depositorCode,
        string packagingCirculationCode,
        string customerNumber,
        string name,
        bool mustBeWashed,
        decimal weight,
        Dimensions dimensions,
        WashingMachineSpeed[] washingMachineSpeeds,
        Guid? id = null
        ) : base(id ?? Guid.NewGuid())
    {
        Code = code.ToUpperInvariant();
        PackagingTypeCode = packagingTypeCode.ToUpperInvariant();
        DepositorCode = depositorCode.ToUpperInvariant();
        PackagingCirculationCode = packagingCirculationCode.ToUpperInvariant();
        CustomerNumber = customerNumber;
        Name = name;
        MustBeWashed = mustBeWashed;
        Dimensions = dimensions;
        Weight = weight;
        WashingMachineSpeeds = washingMachineSpeeds;
    }
    
    public void Apply(PackagingCreatedEvent @event)
    {
        Id = @event.Id;
        Code = @event.Code;
        PackagingTypeCode = @event.PackagingTypeCode;
        DepositorCode = @event.DepositorCode;
        PackagingCirculationCode = @event.PackagingCirculationCode;
        CustomerNumber = @event.CustomerNumber;
        Name = @event.Name;
        MustBeWashed = @event.MustBeWashed;
        Weight = @event.Weight;
        Dimensions = @event.Dimensions;
        WashingMachineSpeeds = @event.WashingMachineSpeeds;
    }
    
    public void Apply(PackagingUpdatedEvent @event)
    {
        PackagingTypeCode = @event.PackagingTypeCode;
        PackagingCirculationCode = @event.PackagingCirculationCode;
        Name = @event.Name;
        MustBeWashed = @event.MustBeWashed;
        Weight = @event.Weight;
        Dimensions = @event.Dimensions;
        WashingMachineSpeeds = @event.WashingMachineSpeeds;
    }
    
    public void Apply(PackagingWashingMachineSpeedCreatedEvent @event)
    {
        WashingMachineSpeeds = WashingMachineSpeeds.Concat([@event.WashingMachineSpeed]).ToArray();
    }
    
    public void Apply(PackagingWashingMachineSpeedDeletedEvent @event)
    {
        WashingMachineSpeeds = WashingMachineSpeeds
            .Where(x => x.WashingMachineCode != @event.WashingMachineSpeed.WashingMachineCode)
            .ToArray();
    }
}