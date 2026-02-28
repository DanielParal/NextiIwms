using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Mmo.Settings.Contracts.PackagingCirculations;

public record CreatePackagingCirculationRequest(
    [property: Required] string Code,
    [property: Required] string Name);