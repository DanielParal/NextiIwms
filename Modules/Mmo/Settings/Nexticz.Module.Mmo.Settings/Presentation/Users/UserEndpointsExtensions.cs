using Microsoft.AspNetCore.Routing;

namespace Nexticz.Module.Mmo.Settings.Presentation.Users;

internal static class UserEndpointsExtensions
{
    public static IEndpointRouteBuilder MapUsersEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapGetUsers()
            .MapUpdateUserEndpoint();
    }
}