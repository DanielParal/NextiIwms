using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Nexticz.Module.Auth.Presentation.Endpoints.Authentications;

public static class LoginPasswordlessEndpoint
{
    public static IEndpointRouteBuilder MapLoginPasswordless(this IEndpointRouteBuilder builder)
    {
        builder.MapPost(ApiEndpoints.Authentications.LoginPasswordless, (CancellationToken cancellationToken) =>
        {
            return "loginPasswordless";
        }).HasApiVersion(1.0)
            .WithName(nameof(ApiEndpoints.Authentications.LoginPasswordless));
        
        return builder;
    }
}