using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Portal.Presentation.Modules;
using Nexticz.Module.Portal.Presentation.Seeds;
using Nexticz.Lib.Shared.Helpers;

namespace Nexticz.Module.Portal.Presentation;

internal static class EndpointsExtensions
{
    public static IEndpointRouteBuilder MapPortalModulesEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.NewVersionedApi(nameof(PortalEndpoints.ModuleEndpoints))
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                    context,
                    [nameof(AuthorizationHelper.Role.Developer), nameof(AuthorizationHelper.Role.SysAdmin)],
                    [nameof(AuthorizationHelper.Permission.Any)])
                )
            )
            .MapModulesEndpoints();

        return builder;
    }
    
    public static IEndpointRouteBuilder MapPortalModulesAnyPermissionEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.NewVersionedApi(nameof(PortalEndpoints.ModuleEndpoints))
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                    context,
                    [nameof(AuthorizationHelper.Role.Any)],
                    [nameof(AuthorizationHelper.Permission.Any)])
                )
            )
            .MapModulesAnyPermissionEndpoints();

        return builder;
    }
    
    public static IEndpointRouteBuilder MapPortalSeedsEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.NewVersionedApi(nameof(PortalEndpoints.SeedEndpoints))
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                    context,
                    [nameof(AuthorizationHelper.Role.Developer)],
                    [nameof(AuthorizationHelper.Permission.Any)])
                )
            )
            .MapSeedsEndpoints();

        return builder;
    }
}