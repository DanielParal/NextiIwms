using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Nexticz.Lib.Shared.Errors;
using Nexticz.Lib.Shared.Extensions;
using Nexticz.Lib.Shared.Helpers;
using StringHelper = Nexticz.Lib.Shared.Helpers.StringHelper;

namespace Nexticz.Module.Auth.Application.Middlewares;

public class AuthenticationAuthorizationMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        if (await IsEndpointPublic(context))
            return;

        if (await IsAuthenticationSchema(StringHelper.AuthenticationSchema.ApiKeySchema, context))
            return;

        if (await IsAuthenticationSchema(StringHelper.AuthenticationSchema.BasicAuthenticationSchema, context))
            return;

        if (await IsAuthenticationSchema(StringHelper.AuthenticationSchema.JwtBearerSchema, context))
            return;

        if (await IsAuthenticationSchema(
                StringHelper.AuthenticationSchema.JwtBearerWithRefreshTokenInHttpOnlyCookieSchema, context))
            return;

        await ReturnUnauthorizedResponse(context);
    }

    private async Task<bool> IsEndpointPublic(HttpContext context)
    {
        var endpoint = context.GetEndpoint();

        var authorize = endpoint?.Metadata.GetMetadata<IAuthorizeData>();
        var allowAnonymous = endpoint?.Metadata.GetMetadata<IAllowAnonymous>();

        var isPublic = authorize == null || allowAnonymous != null;

        if (isPublic) 
            await next(context);

        return isPublic;
    }

    private async Task<bool> IsAuthenticationSchema(string schemaName, HttpContext context)
    {
        var result = await context.AuthenticateAsync(schemaName);
        if (!result.Succeeded)
            return false;

        context.User = result.Principal;

        if (!IsAuthorized(context))
            return false;

        await next(context);
        return true;
    }

    private static bool IsAuthorized(HttpContext context)
    {
        var roles =
            context.Items.FirstOrDefault(x => (string)x.Key == StringHelper.Claim.Type.MagicRoles)
                .Value as List<string> ?? [];
        var permissions =
            context.Items.FirstOrDefault(x => (string)x.Key == StringHelper.Claim.Type.MagicPermissions)
                .Value as List<string> ?? [];

        if (AuthorizationHelper.HasUserAccessRights(context, roles, permissions)) 
            return true;

        context.Items.Add(new KeyValuePair<object, object?>("Authorization", false));
           
        return false;
    }

    private static async Task ReturnUnauthorizedResponse(HttpContext context)
    {
        var option = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters =
            {
                new JsonStringEnumConverter()
            }
        };

        context.Response.ContentType = "application/json";

        var authorization = context.Items.FirstOrDefault(x => (string)x.Key == "Authorization").Value;

        if (authorization is not null)
        {
            context.Response.Headers.Append(StringHelper.Header.XAuthorizationFailed,
                "Authorization invalid. You don't have the required permissions.");
            context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
        }
        else
        {
            context.Response.Cookies.Delete(StringHelper.Header.XAccessToken);
            context.Response.Cookies.Delete(StringHelper.Header.XRefreshToken);
            context.Response.Headers.Append(StringHelper.Header.XAuthorizationFailed,
                "Authentication invalid. Please re-authenticate.");
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
        }

        var errorResult =
            JsonSerializer.Serialize(Errors.Common.Unauthorize.MapToErrorResponse(), option);

        await context.Response.WriteAsync(errorResult);
    }
}