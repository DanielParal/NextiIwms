namespace Nexticz.Module.Auth.Contracts.Authentications;

public record EmailConfirmationRequest
{
    public required string Token { get; set; }
    public required string Email { get; set; }
}