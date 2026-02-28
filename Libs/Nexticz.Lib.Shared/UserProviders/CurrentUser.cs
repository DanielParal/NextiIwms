namespace Nexticz.Lib.Shared.UserProviders;

public record CurrentUser
{
    public required Guid Id { get; set; }
    public required string UserName { get; set; }
    public string? Email { get; set; }
    public IReadOnlyList<string> Roles { get; set; } = null!;
    public IReadOnlyList<string> Permissions { get; set; } = null!;
    public required string UserDeviceInfo { get; set; } = null!;
    public required string XApiLanguage { get; set; }
}