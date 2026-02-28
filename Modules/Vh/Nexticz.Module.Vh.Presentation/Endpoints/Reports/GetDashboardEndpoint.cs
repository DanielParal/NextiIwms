using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Vh.Application.ReportActivities.Common.Models;
using Nexticz.Module.Vh.Application.Reports.Queries.GetDashboard;
using Nexticz.Module.Vh.Contracts.Reports;
using Nexticz.Module.Vh.Contracts.VhUsers;
using Nexticz.Lib.Shared.Helpers;

namespace Nexticz.Module.Vh.Presentation.Endpoints.Reports;

public static class GetDashboardEndpoint
{
    public static IEndpointRouteBuilder MapDashboard(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(ApiEndpoints.Reports.GetDashboard,
                async ([AsParameters] ReportDashboardFilteringParams filteringParams,
                    ISender mediatr, CancellationToken cancellationToken) =>
                {
                    var query = new GetDashboardQuery(filteringParams);
                    var result = await mediatr.Send(query, cancellationToken);

                    return result.Match(
                        Results.Ok, ResultsHelper.Problem);
                })
            .Produces<DashboardResponse>()
            .HasApiVersion(1.0)
            .WithName(nameof(ApiEndpoints.Reports.GetDashboard))
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                    context,
                    [nameof(VhRole.VhMember)],
                    [nameof(VhPermission.VhReportsDashboard)])
                )
            );

        return builder;
    }
}