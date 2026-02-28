using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Mmo.Settings.Contracts.KitTypes;

public record CreateKitTypeRequest(
    [property: Required] string Code, 
    [property: Required] string Name);