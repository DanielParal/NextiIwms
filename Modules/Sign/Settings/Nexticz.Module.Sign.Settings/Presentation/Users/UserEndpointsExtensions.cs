using Microsoft.AspNetCore.Routing;

namespace Nexticz.Module.Sign.Settings.Presentation.Users;

internal static class UserEndpointsExtensions
{
    public static IEndpointRouteBuilder MapUsersEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapGetUsersEndpoint()
            .MapUpdateUserEndpoint()
            .MapUploadUserSignatureEndpoint()
            .MapGetUserSignatureFileEndpoint()
            .MapDeleteUserSignatureEndpoint();
    }
}