namespace Nexticz.Module.Auth.Contracts.RefreshTokens;

public class RefreshTokenResponse
{
    public Guid Id { get; set; }
    public required string UserDeviceInfo { get; set; }
    public required string Value { get; set; }
    public DateTime Created { get; set; } = DateTime.UtcNow;
    public DateTime LastActivity { get; set; } = DateTime.UtcNow;
    public DateTime? Expiration { get; set; }
    public required Guid UserId { get; set; }
}