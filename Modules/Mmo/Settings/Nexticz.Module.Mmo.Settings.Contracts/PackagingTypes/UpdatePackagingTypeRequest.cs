using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Mmo.Settings.Contracts.PackagingTypes;

public record UpdatePackagingTypeRequest(
    [property: Required] string Name);