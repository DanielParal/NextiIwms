using System.Text.Encodings.Web;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Auth.Application.Authentications.Commands.RefreshAccessToken;
using Nexticz.Module.Auth.Application.Common.Interfaces;
using IAuthenticationService = Nexticz.Module.Auth.Application.Common.Interfaces.IAuthenticationService;
using Interfaces_IAuthenticationService = Nexticz.Module.Auth.Application.Common.Interfaces.IAuthenticationService;

namespace Nexticz.Module.Auth.Application.Extensions;

public class JwtBearerWithRefreshTokenInHttpOnlyCookieAuthenticationSchemaHandler(
    Interfaces_IAuthenticationService authenticationService,
    IJwtTokenGenerator jwtTokenGenerator,
    ISender mediatr,
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        Request.Cookies.TryGetValue(StringHelper.Header.XAccessToken, out var accessToken);
        Request.Cookies.TryGetValue(StringHelper.Header.XRefreshToken, out var refreshToken);

        if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(refreshToken))
            return AuthenticateResult.Fail("Authentication failed");

        var principal = jwtTokenGenerator.ValidateAccessToken(accessToken, true);

        if (principal is not null)
            return AuthenticateResult.Success(new AuthenticationTicket(principal, Scheme.Name));

        var command = new RefreshAccessTokenCommand { AccessToken = accessToken, RefreshToken = refreshToken };
        var result = await mediatr.Send(command);

        if (result.IsError)
            return AuthenticateResult.Fail("Authentication failed");

        principal = jwtTokenGenerator.ValidateAccessToken(result.Value.AccessToken, true);

        if (principal is null)
            return AuthenticateResult.Fail("Authentication failed");
        
        authenticationService.AddResponseAuthorizationCookies(result.Value.AccessToken, null!);

        return AuthenticateResult.Success(new AuthenticationTicket(principal, Scheme.Name));
    }
}