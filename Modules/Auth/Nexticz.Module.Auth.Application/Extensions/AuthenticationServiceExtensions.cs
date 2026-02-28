using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nexticz.Lib.Shared.Helpers;

namespace Nexticz.Module.Auth.Application.Extensions;

public static class AuthenticationServiceExtensions
{
    public static IServiceCollection AddAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication()
            .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationSchemaHandler>(
                StringHelper.AuthenticationSchema.ApiKeySchema, null)
            .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationSchemaHandler>(
                StringHelper.AuthenticationSchema.BasicAuthenticationSchema, null)
            .AddScheme<AuthenticationSchemeOptions, JwtBearerAuthenticationSchemaHandler>(
                StringHelper.AuthenticationSchema.JwtBearerSchema, null)
            .AddScheme<AuthenticationSchemeOptions,
                JwtBearerWithRefreshTokenInHttpOnlyCookieAuthenticationSchemaHandler>(
                StringHelper.AuthenticationSchema.JwtBearerWithRefreshTokenInHttpOnlyCookieSchema, null);

        return services;
    }
}