namespace Nexticz.Module.Auth.Contracts.ApiKeys;

public class ApiKeyResponse
{
    public required Guid Id { get; set; }
    public required Guid UserId { get; set; }
    public required DateTime Created { get; set; }
    public required DateTime? LastActivity { get; set; }
    public required DateTime? Expiration { get; set; }
    public required string Description { get; set; }
    public required string Value { get; set; }
}