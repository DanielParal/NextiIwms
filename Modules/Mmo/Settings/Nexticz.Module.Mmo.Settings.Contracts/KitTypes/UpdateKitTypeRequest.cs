using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Mmo.Settings.Contracts.KitTypes;

public record UpdateKitTypeRequest([property: Required] string Name);