using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Vh.Application.ReportActivities.Common.Models;
using Nexticz.Module.Vh.Application.Reports.Queries.GetReportActivities;
using Nexticz.Module.Vh.Contracts.Reports;
using Nexticz.Module.Vh.Contracts.VhUsers;
using Nexticz.Module.Vh.Domain.ReportActivities;
using Nexticz.Module.Vh.Presentation.Endpoints.Reports.Mappers;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Lib.Shared.Helpers;


namespace Nexticz.Module.Vh.Presentation.Endpoints.Reports;

public static class GetReportActivitiesEndpoint
{
    public static IEndpointRouteBuilder MapGetReportActivities(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(ApiEndpoints.Reports.GetReportActivities,
                async ([AsParameters] ReportActivitiesFilteringParams filteringParams, ISender mediatr,
                    CancellationToken cancellationToken) =>
                {
                    var query = new GetReportActivitiesQuery { FilteringParams = filteringParams };
                    var result = await mediatr.Send(query, cancellationToken);

                    if (!result.IsError && result.Value.data.GetType() == typeof(ReportActivity))
                        result.Value.data = result.Value.data.OfType<ReportActivity>()
                            .Select(x => x.MapToReportActivitiesResponse());

                    return result.Match(
                        Results.Ok,
                        ResultsHelper.Problem);
                })
            .Produces<FilteredResult<ReportActivityResponse>>()
            .HasApiVersion(1.0)
            .WithName(nameof(ApiEndpoints.Reports.GetReportActivities))
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                    context,
                    [nameof(VhRole.VhMember)],
                    [nameof(VhPermission.VhReportsReportActivities)])
                )
            );

        return builder;
    }
}