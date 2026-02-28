namespace Nexticz.Module.Auth.Contracts.ApiKeys;

public class CreateApiKeyRequest
{
    public required Guid UserId { get; set; }
    public required string Description { get; set; }
    public DateTime? Expiration { get; set; }
}