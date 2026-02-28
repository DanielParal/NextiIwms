using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Mmo.Settings.Contracts.KitSapDefinitions;

public record UpdateKitSapDefinitionRequest([property: Required] string Name);