using Microsoft.AspNetCore.Routing;

namespace Nexticz.Module.Auth.Presentation.Endpoints.ApiKeys;

public static class ApiKeysExtensions
{
    public static IEndpointRouteBuilder MapApiKeysEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapGetApiKeysByUserId()
            .MapCreateApiKey()
            .MapUpdateApiKey()
            .MapDeleteApiKey();
    }
}