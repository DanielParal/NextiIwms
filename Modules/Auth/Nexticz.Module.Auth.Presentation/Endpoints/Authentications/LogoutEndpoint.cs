using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Auth.Application.Authentications.Commands.Logout;
using Nexticz.Module.Auth.Application.Common.Interfaces;
using Nexticz.Lib.Shared.Helpers;

namespace Nexticz.Module.Auth.Presentation.Endpoints.Authentications;

public static class LogoutEndpoint
{
    public static IEndpointRouteBuilder MapLogout(this IEndpointRouteBuilder builder)
    {
        builder.MapPost(ApiEndpoints.Authentications.Logout, async (IAuthenticationService authenticationService,
                HttpContext httpContext, ISender mediatr, CancellationToken cancellationToken) =>
            {
                var accessToken = httpContext.Request.Cookies[StringHelper.Header.XAccessToken];
                var refreshToken = httpContext.Request.Cookies[StringHelper.Header.XRefreshToken];

                if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(refreshToken))
                {
                    authenticationService.DeleteResponseAuthorizationCookies();
                    return Results.NoContent();
                }

                var command = new LogoutCommand { AccessToken = accessToken, RefreshToken = refreshToken };
                var result = await mediatr.Send(command, cancellationToken);

                authenticationService.DeleteResponseAuthorizationCookies();

                return result.Match(
                    _ => Results.NoContent(),
                    ResultsHelper.Problem);
            }).HasApiVersion(1.0)
            .WithName(nameof(ApiEndpoints.Authentications.Logout));

        return builder;
    }
}