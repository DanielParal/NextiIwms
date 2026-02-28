using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;
using Nexticz.Module.Mmo.SharedKernel;
using Nexticz.Module.Mmo.SharedKernel.DomainCore;

namespace Nexticz.Module.Mmo.Settings.Domain.PackagingAggregate.Events;

public class PackagingWashingMachineSpeedDeletedEvent(
    Guid id,
    string code,
    WashingMachineSpeed washingMachineSpeed) : EventWithCode(code), IMartenEvent
{
    public Guid Id { get; } = id;
    public WashingMachineSpeed WashingMachineSpeed { get; } = washingMachineSpeed;
    
}