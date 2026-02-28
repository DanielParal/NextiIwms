using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Mmo.Settings.Contracts.PackagingCirculations;

public record UpdatePackagingCirculationRequest(
    [property: Required] string Name);