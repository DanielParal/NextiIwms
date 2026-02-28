using System.Text;
using System.Text.Encodings.Web;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nexticz.Module.Auth.Contracts.Authentications;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Auth.Application.Authentications.Queries.Login;
using Nexticz.Module.Auth.Application.Common.Interfaces;

namespace Nexticz.Module.Auth.Application.Extensions;

public class BasicAuthenticationSchemaHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ISender mediatr,
    IJwtTokenGenerator jwtTokenGenerator,
    ILoggerFactory logger,
    UrlEncoder encoder)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        Request.Headers.TryGetValue(StringHelper.Header.Authorization, out var authorization);

        if (string.IsNullOrEmpty(authorization) || !authorization.ToString().StartsWith("Basic"))
            return AuthenticateResult.Fail("Authentication failed");

        var encodedCredentials = authorization.ToString().Split(" ", 2)[1];

        var decodedCredentials = Encoding.UTF8.GetString(Convert.FromBase64String(encodedCredentials));

        var parts = decodedCredentials.Split(':', 2);

        var loginQuery = new LoginQuery
            { LoginRequest = new LoginRequest { Username = parts[0], Password = parts[1] } };

        var result = await mediatr.Send(loginQuery);

        if (result.IsError)
            return AuthenticateResult.Fail("Authentication failed");

        var principal = jwtTokenGenerator.ValidateAccessToken(result.Value.AccessToken, true);

        return principal is null
            ? AuthenticateResult.Fail("Authentication failed")
            : AuthenticateResult.Success(new AuthenticationTicket(principal, Scheme.Name));
    }
}