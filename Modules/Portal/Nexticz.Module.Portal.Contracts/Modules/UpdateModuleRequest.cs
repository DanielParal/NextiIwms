using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Portal.Contracts.Modules;

public record UpdateModuleRequest(
    [property: Required] string Name, 
    [property: Required] string Icon, 
    [property: Required] string BaseUrl, 
    [property: Required] bool IsActive);
