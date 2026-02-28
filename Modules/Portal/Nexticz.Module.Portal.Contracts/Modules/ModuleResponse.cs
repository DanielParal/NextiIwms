using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Portal.Contracts.Modules;

public class ModuleResponse
{
    [property: Required] public Guid Id { get; set; }
    [property: Required] public string Name { get; set; }
    [property: Required] public string Icon { get; set; }
    [property: Required] public string BaseUrl { get; set; }
    [property: Required] public bool IsActive { get; set; }
    [property: Required] public int SortOrder { get; set; }
}