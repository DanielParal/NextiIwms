using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Vh.Application.Reports.Queries.GetReportWorkerShiftPerformance;
using Nexticz.Module.Vh.Contracts.Reports;
using Nexticz.Module.Vh.Contracts.VhUsers;
using Nexticz.Lib.Shared.Helpers;

namespace Nexticz.Module.Vh.Presentation.Endpoints.Reports;

public static class GetReportWorkerShiftPerformanceEndpoint
{
    public static IEndpointRouteBuilder MapGetReportWorkerShiftPerformance(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(ApiEndpoints.Reports.GetReportWorkerShiftPerformance,
                async ([AsParameters] PerformanceReportsRequest performanceReportsRequest, ISender mediatr,
                    CancellationToken cancellationToken) =>
                {
                    var query = new GetReportWorkerShiftPerformanceQuery(performanceReportsRequest);
                    var result = await mediatr.Send(query, cancellationToken);

                    return result.Match(
                        Results.Ok,
                        ResultsHelper.Problem);
                })
            .Produces<ReportWorkerShiftsPerformanceResponse>()
            .HasApiVersion(1.0)
            .WithName(nameof(ApiEndpoints.Reports.GetReportWorkerShiftPerformance))
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                    context,
                    [nameof(VhRole.VhMember)],
                    [nameof(VhPermission.VhReportsShiftPerformance)])
                )
            );

        return builder;
    }
}