using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;
using Nexticz.Module.Mmo.SharedKernel;
using Nexticz.Module.Mmo.SharedKernel.DomainCore;

namespace Nexticz.Module.Mmo.Settings.Domain.KitAggregate.Events;

public class KitUpdatedEvent(
    Guid id,
    string code,
    string manufactureCode,
    string kitSapDefinitionCode,
    string note,
    string definingPackagingCode,
    int dryingTime,
    bool hasKitInstructionFile,
    PackagingCodeQuantity[] packagingCodeQuantities,
    SpecialInformationSchedule[] specialInformationSchedules) : EventWithCode(code), IMartenEvent
{
    public Guid Id { get; } = id;
    public string ManufactureCode { get; } = manufactureCode.ToUpperInvariant();
    public string KitSapDefinitionCode { get; } = kitSapDefinitionCode.ToUpperInvariant();
    public string Note { get; } = note;
    public string DefiningPackagingCode { get; } = definingPackagingCode.ToUpperInvariant();
    public int DryingTime { get; } = dryingTime;
    public bool HasKitInstructionFile { get; } = hasKitInstructionFile;
    public PackagingCodeQuantity[] PackagingCodeQuantities { get; } = packagingCodeQuantities;
    public SpecialInformationSchedule[] SpecialInformationSchedules { get; } = specialInformationSchedules;
}