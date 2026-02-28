using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Sign.Settings.Contracts.Locations;

public record LocationResponse(
    [property: Required] Guid Id, 
    [property: Required] string Code, 
    [property: Required] string Name);