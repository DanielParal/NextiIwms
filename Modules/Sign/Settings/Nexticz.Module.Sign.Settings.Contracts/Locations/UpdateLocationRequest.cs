using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Sign.Settings.Contracts.Locations;

public record UpdateLocationRequest([property: Required] string Name);