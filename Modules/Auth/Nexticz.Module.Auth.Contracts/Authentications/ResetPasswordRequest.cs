namespace Nexticz.Module.Auth.Contracts.Authentications;

public class ResetPasswordRequest
{
    public required string Email { get; set; }
    public required string Token { get; set; }
    public required string Password { get; set; }
}