namespace Nexticz.Module.Auth.Contracts.Authentications;

public record LoginRequest
{
    public required string Username { get; set; }
    public required string Password { get; set; }
}