using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Mmo.Settings.Contracts.KitTypes;

public record KitTypeResponse(
    [property: Required] Guid Id, 
    [property: Required] string Code, 
    [property: Required] string Name);