using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Auth.Application.Authentications.Common;
using Nexticz.Module.Auth.Application.Authentications.Queries.Login;
using Nexticz.Module.Auth.Application.Common.Interfaces;
using Nexticz.Module.Auth.Contracts.Authentications;
using Nexticz.Lib.Shared.Helpers;

namespace Nexticz.Module.Auth.Presentation.Endpoints.Authentications;

public static class LoginEndpoint
{
    public static IEndpointRouteBuilder MapLogin(this IEndpointRouteBuilder builder)
    {
        builder.MapPost(ApiEndpoints.Authentications.Login, async (LoginRequest request, ISender mediatr,
                CancellationToken cancellationToken, IAuthenticationService authenticationService,
                HttpContext httpContext) =>
            {
                var accessToken = httpContext.Request.Cookies[StringHelper.Header.XAccessToken];
                var refreshToken = httpContext.Request.Cookies[StringHelper.Header.XRefreshToken];

                if (!string.IsNullOrEmpty(accessToken) || !string.IsNullOrEmpty(refreshToken))
                {
                    authenticationService.DeleteResponseAuthorizationCookies();
                    return ResultsHelper.Problem(AuthenticationErrors.UserIsCurentlyLogged);
                }

                var loginQuery = new LoginQuery { LoginRequest = request };
                var result = await mediatr.Send(loginQuery, cancellationToken);

                if (!result.IsError)
                    authenticationService.AddResponseAuthorizationCookies(result.Value.AccessToken,
                        result.Value.RefreshToken);

                return result.Match(
                    Results.Ok,
                    ResultsHelper.Problem);
            })
            .Produces<AuthenticationResponse>()
            .HasApiVersion(1.0)
            .WithName(nameof(ApiEndpoints.Authentications.Login));

        return builder;
    }
}