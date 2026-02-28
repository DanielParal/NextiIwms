namespace Nexticz.Module.Auth.Contracts.Authentications;

public record AuthenticationResponse
{
    public Guid Id { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Firsname { get; set; }
    public string? Lastname { get; set; }
    public string? Company { get; set; }
    public required string AccessToken { get; set; }
    public required string RefreshToken { get; set; }
};