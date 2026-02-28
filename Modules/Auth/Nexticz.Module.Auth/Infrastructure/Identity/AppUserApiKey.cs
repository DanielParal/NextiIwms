namespace Nexticz.Module.Auth.Infrastructure.Identity;

public class AppUserApiKey
{
    public Guid Id { get; set; }
    public required string Description { get; set; }
    public required string Value { get; set; }
    public DateTime Created { get; set; } = DateTime.UtcNow;
    public DateTime? LastActivity { get; set; }
    public DateTime? Expiration { get; set; }
    public required Guid UserId { get; set; }
    public AppUser? AppUser { get; set; }
}