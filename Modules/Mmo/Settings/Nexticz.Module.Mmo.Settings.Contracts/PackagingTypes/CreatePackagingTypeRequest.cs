using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Mmo.Settings.Contracts.PackagingTypes;

public record CreatePackagingTypeRequest(
    [property: Required] string Code,
    [property: Required] string Name);