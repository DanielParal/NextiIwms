namespace Nexticz.Module.Auth.Contracts.ApiKeys;

public class UpdateApiKeyRequest
{
    public required string Description { get; set; }
    public DateTime? Expiration { get; set; }
}