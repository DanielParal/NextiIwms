using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Mmo.Planning.Presentation.WashingMachines;
using Nexticz.Module.Mmo.SharedKernel.Security;
using Nexticz.Lib.Shared.Helpers;

namespace Nexticz.Module.Mmo.Planning.Presentation;

internal static class EndpointsExtensions
{
    public static IEndpointRouteBuilder MapMmoWashingMachinesEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.NewVersionedApi(nameof(PlanningEndpoints.WashingMachineEndpoints))
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                    context,
                    [nameof(Role.MmoMember)],
                    [nameof(Permission.MmoManageAll), nameof(Permission.MmoManagePlanning), nameof(Permission.MmoMaster), nameof(Permission.MmoForkliftLoader)])
                )
            )
            .MapWashingMachineEndpoints();

        builder.NewVersionedApi(nameof(PlanningEndpoints.WashingMachineEndpoints))
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                    context,
                    [nameof(Role.MmoMember)],
                    [nameof(Permission.MmoManageAll), nameof(Permission.MmoManagePlanning), nameof(Permission.MmoMaster), 
                        nameof(Permission.MmoForkliftLoader), nameof(Permission.MmoCompletion)])
                )
            )
            .MapGetWashingMachineEndpoints();
        
        return builder;
    }
}