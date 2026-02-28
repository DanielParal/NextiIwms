using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Mmo.Settings.Contracts.Kits;

public record PackagingQuantityContract(
    [property: Required] string PackagingCode, 
    [property: Required] string PackagingTypeName, 
    [property: Required] int Quantity);