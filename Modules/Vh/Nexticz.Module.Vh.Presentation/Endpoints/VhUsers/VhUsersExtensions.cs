using Microsoft.AspNetCore.Routing;

namespace Nexticz.Module.Vh.Presentation.Endpoints.VhUsers;

public static class VhUsersExtensions
{
    public static IEndpointRouteBuilder MapVhUsersEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapGetVhUsers()
            .MapUpdateVhUserEndpoint();
    }
}