using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Sign.Settings.Contracts.DepositorGroups;

public record DepositorGroupResponse(
    [property: Required] Guid Id, 
    [property: Required] string Code, 
    [property: Required] string Name);