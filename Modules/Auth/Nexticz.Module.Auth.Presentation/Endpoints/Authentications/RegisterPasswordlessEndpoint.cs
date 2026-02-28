using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Nexticz.Module.Auth.Presentation.Endpoints.Authentications;

public static class RegisterPasswordlessEndpoint
{
    public static IEndpointRouteBuilder MapRegisterPasswordless(this IEndpointRouteBuilder builder)
    {
        builder.MapPost(ApiEndpoints.Authentications.RegisterPasswordless, (CancellationToken cancellationToken) =>
        {
            return "registerPasswordless";
        }).HasApiVersion(1.0)
            .WithName(nameof(ApiEndpoints.Authentications.RegisterPasswordless));
        
        return builder;
    }
}