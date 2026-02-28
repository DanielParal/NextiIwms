namespace Nexticz.Module.Auth.Contracts.Accounts;

public record UserChangedMessage(
    Guid Id,
    string Username,
    string? FullName,
    string[] Roles,
    string[] Permissions,
    UserChangeTypeContract UserChangeType);