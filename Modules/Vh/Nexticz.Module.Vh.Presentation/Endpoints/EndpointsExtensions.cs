using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Vh.Contracts.VhUsers;
using Nexticz.Module.Vh.Presentation.Endpoints.ActivityCategories;
using Nexticz.Module.Vh.Presentation.Endpoints.Assortments;
using Nexticz.Module.Vh.Presentation.Endpoints.BandRewards;
using Nexticz.Module.Vh.Presentation.Endpoints.Centers;
using Nexticz.Module.Vh.Presentation.Endpoints.Depositors;
using Nexticz.Module.Vh.Presentation.Endpoints.DepositorsGroups;
using Nexticz.Module.Vh.Presentation.Endpoints.LoadedActivities;
using Nexticz.Module.Vh.Presentation.Endpoints.LoadingActionsNdas;
using Nexticz.Module.Vh.Presentation.Endpoints.LoadingDevices;
using Nexticz.Module.Vh.Presentation.Endpoints.NonDispensingActivities;
using Nexticz.Module.Vh.Presentation.Endpoints.Partners;
using Nexticz.Module.Vh.Presentation.Endpoints.Reports;
using Nexticz.Module.Vh.Presentation.Endpoints.SagDynamicsActivities;
using Nexticz.Module.Vh.Presentation.Endpoints.ShiftMasterChanges;
using Nexticz.Module.Vh.Presentation.Endpoints.SystemActivities;
using Nexticz.Module.Vh.Presentation.Endpoints.VhUsers;
using Nexticz.Module.Vh.Presentation.Endpoints.Workers;
using Nexticz.Module.Vh.Presentation.Endpoints.WorkerShifts;
using Nexticz.Lib.Shared.Helpers;

namespace Nexticz.Module.Vh.Presentation.Endpoints;

public static class EndpointsExtensions
{
    public static IEndpointRouteBuilder MapVhApiEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.NewVersionedApi(nameof(ApiEndpoints.VhUsers))
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                    context,
                    [nameof(VhRole.VhMember)],
                    [nameof(AuthorizationHelper.Permission.Any)])
                )
            )
            .MapVhUsersEndpoints();

        builder.NewVersionedApi(nameof(ApiEndpoints.Centers))
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                    context,
                    [nameof(VhRole.VhMember)],
                    [nameof(AuthorizationHelper.Permission.Any)])
                )
            )
            .MapCentersEndpoints();

        builder.NewVersionedApi(nameof(ApiEndpoints.Depositors))
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                    context,
                    [nameof(VhRole.VhMember)],
                    [nameof(AuthorizationHelper.Permission.Any)])
                )
            )
            .MapDepositorsEndpoints();

        builder.NewVersionedApi(nameof(ApiEndpoints.DepositorsGroups))
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                    context,
                    [nameof(VhRole.VhMember)],
                    [nameof(AuthorizationHelper.Permission.Any)])
                )
            )
            .MapDepositorsGroupsEndpoints();

        builder.NewVersionedApi(nameof(ApiEndpoints.Workers))
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                    context,
                    [nameof(VhRole.VhMember)],
                    [nameof(AuthorizationHelper.Permission.Any)])
                )
            )
            .MapWorkersEndpoints();

        builder.NewVersionedApi(nameof(ApiEndpoints.ActivityCategories))
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                    context,
                    [nameof(VhRole.VhMember)],
                    [nameof(AuthorizationHelper.Permission.Any)])
                )
            )
            .MapActivityCategoriesEndpoints();

        builder.NewVersionedApi(nameof(ApiEndpoints.NonDispensingActivities))
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                    context,
                    [nameof(VhRole.VhMember)],
                    [nameof(AuthorizationHelper.Permission.Any)])
                )
            )
            .MapNonDispensingActivitiesEndpoints();

        builder.NewVersionedApi(nameof(ApiEndpoints.SystemActivities))
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                    context,
                    [nameof(VhRole.VhMember)],
                    [nameof(AuthorizationHelper.Permission.Any)])
                )
            )
            .MapSystemActivitiesEndpoints();

        builder.NewVersionedApi(nameof(ApiEndpoints.Assortments))
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                    context,
                    [nameof(VhRole.VhMember)],
                    [nameof(AuthorizationHelper.Permission.Any)])
                )
            )
            .MapAssortmentsEndpoints();

        builder.NewVersionedApi(nameof(ApiEndpoints.BandRewards))
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                    context,
                    [nameof(VhRole.VhMember)],
                    [nameof(AuthorizationHelper.Permission.Any)])
                )
            )
            .MapGetBandRewardsEndpoints();

        builder.NewVersionedApi(nameof(ApiEndpoints.Partners))
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                    context,
                    [nameof(VhRole.VhMember)],
                    [nameof(AuthorizationHelper.Permission.Any)])
                )
            )
            .MapPartnersEndpoints();

        builder.NewVersionedApi(nameof(ApiEndpoints.LoadingDevices))
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                    context,
                    [nameof(VhRole.VhMember)],
                    [nameof(AuthorizationHelper.Permission.Any)])
                )
            )
            .MapLoadingDevicesEndpoints();

        builder.NewVersionedApi(nameof(ApiEndpoints.LoadingActionsNdas))
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                    context,
                    [nameof(VhRole.VhMember)],
                    [nameof(AuthorizationHelper.Permission.Any)])
                )
            )
            .MapLoadingActionsNdasEndpoints();

        builder.NewVersionedApi(nameof(ApiEndpoints.WorkerShifts))
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                    context,
                    [nameof(VhRole.VhMember)],
                    [nameof(AuthorizationHelper.Permission.Any)])
                )
            )
            .MapWorkerShiftsEndpoints();

        builder.NewVersionedApi(nameof(ApiEndpoints.ShiftMasterChanges))
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                    context,
                    [nameof(VhRole.VhMember)],
                    [nameof(AuthorizationHelper.Permission.Any)])
                )
            )
            .MapShiftMasterChangesEndpoints();

        builder.NewVersionedApi(nameof(ApiEndpoints.Reports))
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                    context,
                    [nameof(VhRole.VhMember)],
                    [nameof(AuthorizationHelper.Permission.Any)])
                )
            )
            .MapReportActivitiesEndpoints();

        builder.NewVersionedApi(nameof(ApiEndpoints.SagDynamicsActivities))
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                    context,
                    [nameof(VhRole.VhMember)],
                    [nameof(AuthorizationHelper.Permission.Any)])
                )
            )
            .MapSagDynamicsActivitiesEndpoints();

        builder.NewVersionedApi(nameof(ApiEndpoints.LoadedActivities))
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                    context,
                    [nameof(VhRole.VhMember)],
                    [nameof(AuthorizationHelper.Permission.Any)])
                )
            )
            .MapLoadedActivitiesEndpoints();

        return builder;
    }
}