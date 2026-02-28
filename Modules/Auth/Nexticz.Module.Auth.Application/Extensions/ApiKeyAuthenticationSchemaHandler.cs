using System.Text.Encodings.Web;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nexticz.Module.Auth.Domain.AppUsers;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Auth.Application.ApiKeys.Queries.GetApiKeyByValue;
using Nexticz.Module.Auth.Application.Common.Interfaces;

namespace Nexticz.Module.Auth.Application.Extensions;

public class ApiKeyAuthenticationSchemaHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ISender mediatr,
    UserManager<AppUser> userManager,
    IJwtTokenGenerator jwtTokenGenerator,
    ILoggerFactory logger,
    UrlEncoder encoder)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        Request.Headers.TryGetValue(StringHelper.Header.XApiKey, out var apiKey);

        if (string.IsNullOrEmpty(apiKey))
            return AuthenticateResult.Fail("Authentication failed");

        var apiKeyQuery = new GetApiKeyByValueQuery(apiKey!);
        var resultApiKey = await mediatr.Send(apiKeyQuery);

        if (resultApiKey.IsError || resultApiKey.Value is null ||
            (resultApiKey.Value.Expiration is not null && resultApiKey.Value.Expiration < DateTime.UtcNow))
            return AuthenticateResult.Fail("Authentication failed");

        resultApiKey.Value.LastActivity = DateTime.UtcNow;

        var appUser = await userManager.Users
            .Include(x => x.AppUserRoles)!
            .ThenInclude(x => x.AppRole)
            .Include(x => x.AppUserClaims)
            .SingleOrDefaultAsync(x => x.Id == resultApiKey.Value.UserId);

        if (appUser is null || (appUser.BlockedFrom is not null && appUser.BlockedFrom < DateTime.UtcNow))
            return AuthenticateResult.Fail("Authentication failed");

        var jwtToken = jwtTokenGenerator.GenerateAccessToken(appUser);

        var principal = jwtTokenGenerator.ValidateAccessToken(jwtToken, true);

        return principal is null
            ? AuthenticateResult.Fail("Authentication failed")
            : AuthenticateResult.Success(new AuthenticationTicket(principal, Scheme.Name));
    }
}