using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Lib.Shared.UserProviders;
using Nexticz.Module.Auth.Contracts.Me;

namespace Nexticz.Module.Auth.Presentation.Endpoints.Me;

public static class MeEndpoint
{
    public static IEndpointRouteBuilder MapGetMe(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(ApiEndpoints.Me.GetMe, (ICurrentUserProvider currentUserProvider) =>
            {
                var currentUser = currentUserProvider.GetCurrentUser();
                var meResponse = new MeResponse(currentUser.Id, currentUser.UserName, currentUser.Roles.ToArray(), currentUser.Permissions.ToArray());
                return Results.Ok(meResponse);
            })
            .Produces<MeResponse>()
            .HasApiVersion(1.0)
            .WithName(nameof(ApiEndpoints.Me.GetMe));

        return builder;
    }
}