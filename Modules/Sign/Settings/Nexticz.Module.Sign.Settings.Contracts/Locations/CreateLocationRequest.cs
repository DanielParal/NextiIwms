using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Sign.Settings.Contracts.Locations;

public record CreateLocationRequest([property: Required] string Code, [property: Required] string Name);