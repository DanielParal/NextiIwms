using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Mmo.SharedKernel.Security;
using Nexticz.Module.Mmo.Washing.Presentation.Batches;
using Nexticz.Module.Mmo.Washing.Presentation.LastEnteredWorkerOnLines;
using Nexticz.Module.Mmo.Washing.Presentation.Simulations;
using Nexticz.Module.Mmo.Washing.Presentation.WashingMachineSoses;
using Nexticz.Module.Mmo.Washing.Presentation.WashingStates;
using Nexticz.Lib.Shared.Helpers;

namespace Nexticz.Module.Mmo.Washing.Presentation;

internal static class EndpointsExtensions
{
    public static IEndpointRouteBuilder MapMmoBatchesEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.NewVersionedApi(nameof(WashingEndpoints.BatchesEndpoints))
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                    context,
                    [nameof(Role.MmoMember)],
                    [nameof(Permission.MmoManageAll), nameof(Permission.MmoManageWashing), nameof(Permission.MmoMaster), nameof(Permission.MmoCompletion)])
                )
            )
            .MapBatchesEndpoints();

        return builder;
    }
    
    public static IEndpointRouteBuilder MapMmoWashingMachineSosEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.NewVersionedApi(nameof(WashingEndpoints.WashingMachineSosEndpoints))
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                    context,
                    [nameof(Role.MmoMember)],
                    [nameof(Permission.MmoManageAll), nameof(Permission.MmoManageWashing), nameof(Permission.MmoMaster), nameof(Permission.MmoCompletion)])
                )
            )
            .MapWashingMachineSosEndpoints();

        return builder;
    }
    
    
    
    public static IEndpointRouteBuilder MapMmoMapLastEnteredWorkerOnLineEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.NewVersionedApi(nameof(WashingEndpoints.LastEnteredWorkerOnLineEndpoints))
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                    context,
                    [nameof(Role.MmoMember)],
                    [nameof(Permission.MmoManageAll), nameof(Permission.MmoManageWashing), nameof(Permission.MmoMaster), nameof(Permission.MmoCompletion)])
                )
            )
            .MapLastEnteredWorkerOnLineEndpoints();

        return builder;
    }
    
    public static IEndpointRouteBuilder MapMmoWashingStateEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.NewVersionedApi(nameof(WashingEndpoints.WashingStateEndpoints))
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                    context,
                    [nameof(Role.MmoMember)],
                    [nameof(Permission.MmoManageAll), nameof(Permission.MmoManageWashing), nameof(Permission.MmoViewDashboardWashingState), nameof(Permission.MmoMaster), nameof(Permission.MmoDashboards)])
                )
            )
            .MapWashingStatesEndpoints();

        return builder;
    }
    
    public static IEndpointRouteBuilder MapMmoSimulationEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.NewVersionedApi(nameof(WashingEndpoints.SimulationEndpoints))
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                    context,
                    [nameof(AuthorizationHelper.Role.Developer)],
                    [nameof(AuthorizationHelper.Permission.Any)])
                )
            )
            .MapSimulationEndpoints();

        return builder;
    }
}