using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Auth.Contracts.Me;

public record MeResponse(
    [property: Required] Guid Id, 
    [property: Required] string Username,
    [property: Required] string[] Roles, 
    [property: Required] string[] Permissions);