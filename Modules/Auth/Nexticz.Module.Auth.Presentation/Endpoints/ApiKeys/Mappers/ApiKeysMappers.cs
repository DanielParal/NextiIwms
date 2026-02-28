using Nexticz.Module.Auth.Contracts.ApiKeys;
using Nexticz.Module.Auth.Domain.AppUserApiKeys;

namespace Nexticz.Module.Auth.Presentation.Endpoints.ApiKeys.Mappers;

public static class ApiKeysMappers
{
    public static ApiKeyResponse MapToApiKeysResponse(this AppUserApiKey apiKey)
    {
        return new ApiKeyResponse
        {
            Id = apiKey.Id,
            UserId = apiKey.UserId,
            Created = apiKey.Created,
            LastActivity = apiKey.LastActivity,
            Expiration = apiKey.Expiration,
            Description = apiKey.Description,
            Value = apiKey.Value
        };
    }

    public static List<ApiKeyResponse> MapToApiKeysResponse(this List<AppUserApiKey> apiKeys)
    {
        return apiKeys.Select(x => x.MapToApiKeysResponse()).ToList();
    }
}