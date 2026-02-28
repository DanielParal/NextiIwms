using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Auth.Presentation.Endpoints.Accounts;
using Nexticz.Module.Auth.Presentation.Endpoints.ApiKeys;
using Nexticz.Module.Auth.Presentation.Endpoints.Authentications;
using Nexticz.Module.Auth.Presentation.Endpoints.RefreshTokens;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Auth.Presentation.Endpoints.Me;

namespace Nexticz.Module.Auth.Presentation.Endpoints;

public static class EndpointsExtensions
{
    public static IEndpointRouteBuilder MapAuthApiEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.NewVersionedApi(nameof(ApiEndpoints.Authentications))
            .MapAuthenticationsEndpoints();

        builder.NewVersionedApi(nameof(ApiEndpoints.Accounts))
            .RequireAuthorization(policy => policy.RequireAssertion(context =>
                    AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                        context,
                        [nameof(AuthorizationHelper.Role.Developer), nameof(AuthorizationHelper.Role.SysAdmin)],
                        [nameof(AuthorizationHelper.Permission.Any)])
                )
            )
            .MapAccountsEndpoints();

        builder.NewVersionedApi(nameof(ApiEndpoints.ApiKeys))
            .RequireAuthorization(policy => policy.RequireAssertion(context =>
                    AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                        context,
                        [nameof(AuthorizationHelper.Role.Developer), nameof(AuthorizationHelper.Role.SysAdmin)],
                        [nameof(AuthorizationHelper.Permission.Any)])
                )
            )
            .MapApiKeysEndpoints();

        builder.NewVersionedApi(nameof(ApiEndpoints.RefreshTokens))
            .RequireAuthorization(policy => policy.RequireAssertion(context =>
                    AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                        context,
                        [nameof(AuthorizationHelper.Role.Developer), nameof(AuthorizationHelper.Role.SysAdmin)],
                        [nameof(AuthorizationHelper.Permission.Any)])
                )
            )
            .MapRefreshTokensEndpoints();

        builder.NewVersionedApi(nameof(ApiEndpoints.Me))
            .RequireAuthorization(policy => policy.RequireAssertion(context =>
                    AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                        context,
                        [nameof(AuthorizationHelper.Role.Any)],
                        [nameof(AuthorizationHelper.Permission.Any)])
                )
            )
            .MapMeEndpoints();
        
        return builder;
    }
}