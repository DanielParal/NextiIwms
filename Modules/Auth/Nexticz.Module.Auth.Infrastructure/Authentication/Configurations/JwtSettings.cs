namespace Nexticz.Module.Auth.Infrastructure.Authentication.Configurations;

public class JwtSettings
{
    public required string Issuer { get; init; }
    public required string Audience { get; init; }
    public required string TokenSecretKey { get; init; }
    public required string TokenExpirationTime { get; init; }
    public required string ApiKeyExpirationTime { get; init; }
    public required string UserIdClaimType { get; set; }
}