using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Mmo.Settings.Contracts.Manufactures;

public record ManufactureResponse(
    [property: Required] Guid Id, 
    [property: Required] string Code, 
    [property: Required] string Name);