using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Mmo.Settings.Contracts.Kits;

public record KitResponse(
    [property: Required] Guid Id, 
    [property: Required] string Code,
    [property: Required] string KitTypeCode,
    [property: Required] string KitSapDefinitionCode,
    [property: Required] string DepositorCode,
    [property: Required] string ManufactureCode,
    [property: Required] string KitNumber,
    [property: Required] string Note,
    [property: Required] string DefiningPackagingCode,
    [property: Required] int DryingTime,
    [property: Required] bool HasKitInstructionFile,
    [property: Required] PackagingQuantityContract[] PackagingQuantities,
    [property: Required] SpecialInformationScheduleContract[] SpecialInformationSchedules);