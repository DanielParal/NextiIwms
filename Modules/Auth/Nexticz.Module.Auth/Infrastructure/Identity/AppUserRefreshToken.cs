namespace Nexticz.Module.Auth.Infrastructure.Identity;

public class AppUserRefreshToken
{
    public Guid Id { get; set; }
    public required string UserDeviceInfo { get; set; }
    public required string Value { get; set; }
    public DateTime Created { get; set; } = DateTime.UtcNow;
    public DateTime LastActivity { get; set; } = DateTime.UtcNow;
    public DateTime? Expiration { get; set; }
    public required Guid UserId { get; set; }
    public AppUser? AppUser { get; set; }
}