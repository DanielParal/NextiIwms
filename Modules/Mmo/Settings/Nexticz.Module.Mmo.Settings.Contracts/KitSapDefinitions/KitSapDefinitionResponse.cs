using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Mmo.Settings.Contracts.KitSapDefinitions;

public record KitSapDefinitionResponse(
    [property: Required] Guid Id, 
    [property: Required] string Code, 
    [property: Required] string Name);