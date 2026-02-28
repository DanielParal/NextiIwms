using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Portal.Contracts.Modules;

public record ChangeModuleOrderRequest(
    [property: Required] int ToOrder);