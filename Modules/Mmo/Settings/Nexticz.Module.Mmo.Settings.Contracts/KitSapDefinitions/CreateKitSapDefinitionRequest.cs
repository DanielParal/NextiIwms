using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Mmo.Settings.Contracts.KitSapDefinitions;

public record CreateKitSapDefinitionRequest(
    [property: Required] string Code, 
    [property: Required] string Name);