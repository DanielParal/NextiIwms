using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Auth.Application.Authentications.Commands.RefreshAccessToken;
using Nexticz.Module.Auth.Application.Authentications.Common;
using Nexticz.Module.Auth.Application.Common.Interfaces;
using Nexticz.Module.Auth.Contracts.Authentications;
using Nexticz.Lib.Shared.Helpers;

namespace Nexticz.Module.Auth.Presentation.Endpoints.Authentications;

public static class RefreshAccessTokenEndpoint
{
    public static IEndpointRouteBuilder MapRefreshAccessToken(this IEndpointRouteBuilder builder)
    {
        builder.MapPost(ApiEndpoints.Authentications.RefreshAccessToken, async (HttpContext httpContext, 
                ISender mediatr, IAuthenticationService authenticationService, CancellationToken cancellationToken) =>
            {
                var accessToken = httpContext.Request.Cookies[StringHelper.Header.XAccessToken];
                var refreshToken = httpContext.Request.Cookies[StringHelper.Header.XRefreshToken];

                if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(refreshToken))
                {
                    return ResultsHelper.Problem(AuthenticationErrors.ValidationTokenError);
                }
                
                var command = new RefreshAccessTokenCommand {AccessToken = accessToken, RefreshToken = refreshToken};
                var result = await mediatr.Send(command, cancellationToken);

                if (!result.IsError)
                {
                    authenticationService.AddResponseAuthorizationCookies(result.Value.AccessToken, refreshToken);
                }
                else
                {
                    authenticationService.DeleteResponseAuthorizationCookies();
                }

                return result.Match(
                    Results.Ok, 
                    ResultsHelper.Problem);

            })
            .Produces<AuthenticationResponse>()
            .HasApiVersion(1.0)
            .WithName(nameof(ApiEndpoints.Authentications.RefreshAccessToken));
        
        return builder;
    }
}