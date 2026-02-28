namespace Nexticz.Module.Auth.Contracts.Accounts;

public class UpdateAccountRequest
{
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? Firstname { get; set; }
    public string? Lastname { get; set; }
    public string? Company { get; set; }
    public required string Email { get; set; }
    public string? PhoneNumber { get; set; }
    public DateTime? BlockedFrom { get; set; }
    public required string[] Roles { get; set; }
    public string[] Permissions { get; set; } = [];
}