using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Vh.Application.ReportActivities.Common.Models;
using Nexticz.Module.Vh.Application.Reports.Queries.GetReportPerformanceEvaluation;
using Nexticz.Module.Vh.Contracts.Reports;
using Nexticz.Module.Vh.Contracts.VhUsers;
using Nexticz.Lib.Shared.Helpers;

namespace Nexticz.Module.Vh.Presentation.Endpoints.Reports;

public static class GetReportPerformanceEvaluationEndpoint
{
    public static IEndpointRouteBuilder MapReportPerformanceEvaluation(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(ApiEndpoints.Reports.GetReportPerformanceEvaluation,
                async ([AsParameters] ReportPerformanceEvaluationFilteringParams filteringParams,
                    ISender mediatr, CancellationToken cancellationToken) =>
                {
                    var query = new GetReportPerformanceEvaluationQuery(filteringParams);
                    var result = await mediatr.Send(query, cancellationToken);

                    return result.Match(
                        Results.Ok, ResultsHelper.Problem);
                })
            .Produces<ReportPerformanceEvaluationResponse>()
            .HasApiVersion(1.0)
            .WithName(nameof(ApiEndpoints.Reports.GetReportPerformanceEvaluation))
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                    context,
                    [nameof(VhRole.VhMember)],
                    [nameof(VhPermission.VhReportsPerformanceEvaluation)])
                )
            );

        return builder;
    }
}