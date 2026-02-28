using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Mmo.Reporting.Presentation.DriedKits;
using Nexticz.Module.Mmo.Reporting.Presentation.LineItems;
using Nexticz.Module.Mmo.Reporting.Presentation.Shifts;
using Nexticz.Module.Mmo.Reporting.Presentation.ShiftSettings;
using Nexticz.Module.Mmo.SharedKernel.Security;
using Nexticz.Lib.Shared.Helpers;

namespace Nexticz.Module.Mmo.Reporting.Presentation;

internal static class EndpointsExtensions
{
    public static IEndpointRouteBuilder MapMmoDriedKitsEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.NewVersionedApi(nameof(ReportingEndpoints.DriedKitEndpoints))
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                    context,
                    [nameof(Role.MmoMember)],
                    [nameof(Permission.MmoManageAll), nameof(Permission.MmoManageReporting), nameof(Permission.MmoMaster)])
                )
            )
            .MapDriedKitsEndpoints();

        return builder;
    }
    
    public static IEndpointRouteBuilder MapMmoShiftSettingsEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.NewVersionedApi(nameof(ReportingEndpoints.ShiftSettingEndpoints))
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                    context,
                    [nameof(Role.MmoMember)],
                    [nameof(Permission.MmoManageAll), nameof(Permission.MmoManageReporting), nameof(Permission.MmoMaster)])
                )
            )
            .MapShiftSettingsEndpoints();

        return builder;
    }
    
    public static IEndpointRouteBuilder MapMmoShiftsEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.NewVersionedApi(nameof(ReportingEndpoints.ShiftEndpoints))
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                    context,
                    [nameof(Role.MmoMember)],
                    [nameof(Permission.MmoManageAll), nameof(Permission.MmoManageReporting), nameof(Permission.MmoMaster)])
                )
            )
            .MapShiftsEndpoints();

        return builder;
    }
    
    public static IEndpointRouteBuilder MapMmoLineItemsEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.NewVersionedApi(nameof(ReportingEndpoints.LineItemEndpoints))
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                    context,
                    [nameof(Role.MmoMember)],
                    [nameof(Permission.MmoManageAll), nameof(Permission.MmoManageReporting), nameof(Permission.MmoMaster)])
                )
            )
            .MapLineItemsEndpoints();

        return builder;
    }
}