using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Mmo.Settings.Contracts.Manufactures;

public record UpdateManufactureRequest(
    [property: Required] string Name);