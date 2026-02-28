using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Mmo.Drying.Presentation.Kits;
using Nexticz.Module.Mmo.SharedKernel.Security;
using Nexticz.Lib.Shared.Helpers;

namespace Nexticz.Module.Mmo.Drying.Presentation;

internal static class EndpointsExtensions
{
    public static IEndpointRouteBuilder MapMmoKitsEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.NewVersionedApi(nameof(DryingEndpoints.KitEndpoints))
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                    context,
                    [nameof(Role.MmoMember)],
                    [nameof(Permission.MmoManageAll), nameof(Permission.MmoManageDrying), 
                        nameof(Permission.MmoMaster), nameof(Permission.MmoForkliftLoader)])
                )
            )
            .MapKitEndpoints();

        return builder;
    }
    
    public static IEndpointRouteBuilder MapMmoDashboardKitsEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.NewVersionedApi(nameof(DryingEndpoints.KitEndpoints))
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                    context,
                    [nameof(Role.MmoMember)],
                    [nameof(Permission.MmoManageAll), nameof(Permission.MmoManageDrying), nameof(Permission.MmoViewDashboardDrying), 
                        nameof(Permission.MmoMaster), nameof(Permission.MmoDashboards), nameof(Permission.MmoCompletion), nameof(Permission.MmoForkliftLoader)])
                )
            )
            .MapGetAllKitsEndpoints();

        return builder;
    }
}