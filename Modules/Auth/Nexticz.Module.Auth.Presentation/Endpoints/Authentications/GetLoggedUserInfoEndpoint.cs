using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Auth.Application.Authentications.Queries.GetLoggedUserInfo;
using Nexticz.Module.Auth.Application.Common.Interfaces;
using Nexticz.Module.Auth.Contracts.Authentications;
using Nexticz.Lib.Shared.Helpers;

namespace Nexticz.Module.Auth.Presentation.Endpoints.Authentications;

public static class GetLoggedUserInfoEndpoint
{
    public static IEndpointRouteBuilder MapGetLoggedUserInfo(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(ApiEndpoints.Authentications.GetLoggedUserInfo, async (ISender mediatr,
            CancellationToken cancellationToken, IAuthenticationService authenticationService, HttpContext httpContext) =>
        {
            var accessToken = httpContext.Request.Cookies[StringHelper.Header.XAccessToken];
            var refreshToken = httpContext.Request.Cookies[StringHelper.Header.XRefreshToken];

            if (!string.IsNullOrEmpty(accessToken) && !string.IsNullOrEmpty(refreshToken))
            {
                var query = new GetLoggedUserInfoQuery { AccessToken = accessToken, RefreshToken = refreshToken };
                var result = await mediatr.Send(query, cancellationToken);
                    
                if (!result.IsError)
                {
                    authenticationService.AddResponseAuthorizationCookies(result.Value.AccessToken, result.Value.RefreshToken);
                }
                else
                {
                    authenticationService.DeleteResponseAuthorizationCookies();
                }

                return result.Match(
                    Results.Ok,
                    ResultsHelper.Problem);
            }

            return Results.NoContent();
        })
        .Produces<AuthenticationResponse>()
        .HasApiVersion(1.0)
        .WithName(nameof(ApiEndpoints.Authentications.GetLoggedUserInfo));
        
        return builder;
    }
}