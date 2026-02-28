using Nexticz.Module.Auth.Domain.AppUsers;

namespace Nexticz.Module.Auth.Application.Authentications.Common;

public record AuthenticationResult
{
    public AppUser? AppUser { get; set; }
};