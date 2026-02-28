using Nexticz.Module.Mmo.Settings.Domain.KitAggregate.Events;
using Nexticz.Module.Mmo.SharedKernel;
using Nexticz.Module.Mmo.SharedKernel.DomainCore;

namespace Nexticz.Module.Mmo.Settings.Domain.KitAggregate;

public class Kit : AggregateRoot
{
    public string Code { get; private set; }
    public string KitTypeCode { get; private set; }
    public string KitSapDefinitionCode { get; private set; }
    public string DepositorCode { get; private set; }
    public string ManufactureCode { get; private set; }
    public string KitNumber { get; private set; }
    public string Note { get; private set; }
    public string DefiningPackagingCode { get; private set; }
    public int DryingTime { get; private set; } // Time is set in minutes
    public bool HasKitInstructionFile { get; private set; }
    public PackagingCodeQuantity[] PackagingCodeQuantities { get; private set; }
    public SpecialInformationSchedule[] SpecialInformationSchedules { get; private set; }
    
    // We need private constructor due to Marten deserialization
    private Kit() {}
    
    public Kit(
        string code,
        string kitTypeCode,
        string kitSapDefinitionCode,
        string depositorCode,
        string manufactureCode,
        string kitNumber,
        string note,
        string definingPackagingCode,
        int dryingTime,
        bool hasKitInstructionFile,
        PackagingCodeQuantity[] packagingCodeQuantities,
        SpecialInformationSchedule[] specialInformationSchedules,
        Guid? id = null
        ) : base(id ?? Guid.NewGuid())
    {
        if (packagingCodeQuantities.All(x => !x.PackagingCode.Equals(definingPackagingCode, StringComparison.InvariantCultureIgnoreCase)))
        {
            throw new ArgumentException("Packaging codes must contain defining packaging unique code");
        }
        
        Code = code.ToUpperInvariant();
        KitTypeCode = kitTypeCode.ToUpperInvariant();
        KitSapDefinitionCode = kitSapDefinitionCode.ToUpperInvariant();
        DepositorCode = depositorCode.ToUpperInvariant();
        ManufactureCode = manufactureCode.ToUpperInvariant();
        KitNumber = kitNumber;
        Note = note;
        DefiningPackagingCode = definingPackagingCode.ToUpperInvariant();
        DryingTime = dryingTime;
        HasKitInstructionFile = hasKitInstructionFile;
        PackagingCodeQuantities = packagingCodeQuantities;
        SpecialInformationSchedules = specialInformationSchedules;
    }

    public void Apply(KitCreatedEvent @event)
    {
        Id = @event.Id;
        Code = @event.Code;
        KitTypeCode = @event.KitTypeCode;
        KitSapDefinitionCode = @event.KitSapDefinitionCode;
        DepositorCode = @event.DepositorCode;
        ManufactureCode = @event.ManufactureCode;
        KitNumber = @event.KitNumber;
        Note = @event.Note;
        DefiningPackagingCode = @event.DefiningPackagingCode;
        DryingTime = @event.DryingTime;
        PackagingCodeQuantities = @event.PackagingCodeQuantities;
        SpecialInformationSchedules = @event.SpecialInformationSchedules;
    }
    
    public void Apply(KitUpdatedEvent @event)
    {
        Id = @event.Id;
        ManufactureCode = @event.ManufactureCode;
        KitSapDefinitionCode = @event.KitSapDefinitionCode;
        Note = @event.Note;
        DefiningPackagingCode = @event.DefiningPackagingCode;
        DryingTime = @event.DryingTime;
        PackagingCodeQuantities = @event.PackagingCodeQuantities;
        SpecialInformationSchedules = @event.SpecialInformationSchedules;
        HasKitInstructionFile = @event.HasKitInstructionFile;
    }
    
    public void Apply(KitInstructionUploadedEvent @event)
    {
        HasKitInstructionFile = true;
    }
    
    public void Apply(KitInstructionDeletedEvent @event)
    {
        HasKitInstructionFile = false;
    }
}