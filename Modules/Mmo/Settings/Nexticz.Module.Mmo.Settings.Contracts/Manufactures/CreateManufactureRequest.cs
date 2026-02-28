using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Mmo.Settings.Contracts.Manufactures;

public record CreateManufactureRequest(
    [property: Required] string Code,
    [property: Required] string Name);