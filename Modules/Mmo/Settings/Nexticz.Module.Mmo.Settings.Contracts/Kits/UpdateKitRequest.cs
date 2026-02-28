using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Mmo.Settings.Contracts.Kits;

public record UpdateKitRequest(
    [property: Required] string ManufactureCode,
    [property: Required] string KitSapDefinitionCode,
    [property: Required] string Note,
    [property: Required] string DefiningPackagingCode,
    [property: Required] int DryingTime,
    [property: Required] PackagingQuantityContract[] PackagingQuantities,
    [property: Required] SpecialInformationScheduleContract[] SpecialInformationSchedules);