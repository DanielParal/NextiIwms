namespace Nexticz.Module.Auth.Contracts.Authentications;

public record RegisterRequest
{
    public required string Username { get; set; }
    public required string  Password { get; set; }
    public required string  Email { get; set; }
};