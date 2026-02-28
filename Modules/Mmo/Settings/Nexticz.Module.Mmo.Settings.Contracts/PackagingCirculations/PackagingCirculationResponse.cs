using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Mmo.Settings.Contracts.PackagingCirculations;

public record PackagingCirculationResponse(
    [property: Required] Guid Id, 
    [property: Required] string Code, 
    [property: Required] string Name);