using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Auth.Application.Common.Interfaces;

namespace Nexticz.Module.Auth.Application.Extensions;

public class JwtBearerAuthenticationSchemaHandler(
    IJwtTokenGenerator jwtTokenGenerator,
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        Request.Headers.TryGetValue(StringHelper.Header.Authorization, out var authorization);

        if (string.IsNullOrEmpty(authorization) || !authorization.ToString().StartsWith("Bearer"))
            return Task.FromResult(AuthenticateResult.Fail("Authentication failed"));

        var accessToken = authorization.ToString().Split(" ", 2)[1];

        if (string.IsNullOrEmpty(accessToken))
            return Task.FromResult(AuthenticateResult.Fail("Authentication failed"));

        var principal = jwtTokenGenerator.ValidateAccessToken(accessToken, true);

        return principal is not null
            ? Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(principal, Scheme.Name)))
            : Task.FromResult(AuthenticateResult.Fail("Authentication failed"));
    }
}