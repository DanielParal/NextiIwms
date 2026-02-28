using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Mmo.Settings.Contracts.PackagingTypes;

public record PackagingTypeResponse(
    [property: Required] Guid Id, 
    [property: Required] string Code, 
    [property: Required] string Name);