using Microsoft.AspNetCore.Identity;
using Nexticz.Module.Auth.Domain.AppUserApiKeys;
using Nexticz.Module.Auth.Domain.AppUserClaims;
using Nexticz.Module.Auth.Domain.AppUserRefreshTokens;
using Nexticz.Module.Auth.Domain.AppUserRoles;

namespace Nexticz.Module.Auth.Domain.AppUsers;

public sealed class AppUser : IdentityUser<Guid>
{
    public AppUser()
    {
    }

    public AppUser(string username, string email)
    {
        UserName = username;
        Email = email;
        EmailConfirmed = true;
        Registered = DateTime.UtcNow;
    }

    public AppUser(string username, string email, string? phone, string? firstname, string? lastname, string? company,
        DateTime? blockedFrom = null)
    {
        Id = Guid.NewGuid();
        UserName = username;
        Email = email;
        PhoneNumber = phone;
        Firstname = firstname;
        Lastname = lastname;
        Company = company;
        EmailConfirmed = true;
        BlockedFrom = blockedFrom;
        Registered = DateTime.UtcNow;
    }

    public string? Company { get; set; }
    public string? Firstname { get; set; }
    public string? Lastname { get; set; }
    public DateTime Registered { get; set; }
    public DateTime? LastActivity { get; set; }
    public DateTime? BlockedFrom { get; set; }
    public ICollection<AppUserRole>? AppUserRoles { get; set; }
    public ICollection<AppUserClaim>? AppUserClaims { get; set; }
    public ICollection<AppUserRefreshToken> AppUserRefreshTokens { get; set; } = null!;
    public ICollection<AppUserApiKey> AppUserApiKeys { get; set; } = null!;
}