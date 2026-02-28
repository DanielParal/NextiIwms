using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Nexticz.Module.Auth.Presentation.Endpoints.Authentications;

public static class ConfirmEmailCompletionEnpoint
{
    public static IEndpointRouteBuilder MapConfirmEmailCompletion(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(ApiEndpoints.Authentications.ConfirmEmailCompletion,
                (string jwtToken, CancellationToken cancellationToken) =>
                {
                    return "confirmEmailCompletion1";
                }).HasApiVersion(1.0)
            .WithName(nameof(ApiEndpoints.Authentications.ConfirmEmailCompletion));
        
        return builder;
    }
}