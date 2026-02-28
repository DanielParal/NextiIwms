using Nexticz.Lib.Shared.Helpers;

namespace Nexticz.Module.Auth.Contracts.Accounts;

public class AccountResponse
{
    public required Guid Id { get; set; }
    public required string Username { get; set; }
    public string? Firstname { get; set; }
    public string? Lastname { get; set; }
    public string? Company { get; set; }
    public required string Email { get; set; }
    public string? PhoneNumber { get; set; }
    public DateTime? BlockedFrom { get; set; }
    public string? DefaultUrl { get; set; }
    public bool PasswordSetuped { get; set; }
    public required string[] Roles { get; set; }
    public required string[] Permissions { get; set; }
    public required DateTime? LastActivity { get; set; }
    public AuthorizationHelper.Role[] AuthRoles { get; set; } = [];
}