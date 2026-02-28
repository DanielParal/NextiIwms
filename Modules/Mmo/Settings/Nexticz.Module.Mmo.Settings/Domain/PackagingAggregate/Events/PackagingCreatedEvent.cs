using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;
using Nexticz.Module.Mmo.SharedKernel;
using Nexticz.Module.Mmo.SharedKernel.DomainCore;

namespace Nexticz.Module.Mmo.Settings.Domain.PackagingAggregate.Events;

public class PackagingCreatedEvent(
    Guid id,
    string code,
    string packagingTypeCode,
    string depositorCode,
    string packagingCirculationCode,
    string customerNumber,
    string name,
    bool mustBeWashed,
    Dimensions dimensions,
    decimal weight, 
    WashingMachineSpeed[] washingMachineSpeeds) : EventWithCode(code), IMartenEvent
{
    public Guid Id { get; } = id;
    public string PackagingTypeCode { get; } = packagingTypeCode.ToUpperInvariant();
    public string DepositorCode { get; } = depositorCode.ToUpperInvariant();
    public string PackagingCirculationCode { get; } = packagingCirculationCode.ToUpperInvariant();
    public string CustomerNumber { get; } = customerNumber;
    public string Name { get; } = name;
    public bool MustBeWashed { get; } = mustBeWashed;
    public Dimensions Dimensions { get; } = dimensions;
    public decimal Weight { get; } = weight;
    public WashingMachineSpeed[] WashingMachineSpeeds { get; private set; } = washingMachineSpeeds;
}