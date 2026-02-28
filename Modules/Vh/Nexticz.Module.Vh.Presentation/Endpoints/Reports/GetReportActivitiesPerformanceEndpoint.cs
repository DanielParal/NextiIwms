using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Vh.Application.Reports.Queries.GetReportActivitiesPerformance;
using Nexticz.Module.Vh.Contracts.Reports;
using Nexticz.Lib.Shared.Helpers;

namespace Nexticz.Module.Vh.Presentation.Endpoints.Reports;

public static class GetReportActivitiesPerformanceEndpoint
{
    public static IEndpointRouteBuilder MapGetReportActivitiesPerformance(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(ApiEndpoints.Reports.GetReportActivitiesPerformance,
                async ([AsParameters] PerformanceReportsRequest performanceReportsRequest, ISender mediatr, CancellationToken cancelationToken) =>
                {
                    var query = new GetReportActivitiesPerformanceQuery(performanceReportsRequest);
                    var result = await mediatr.Send(query, cancelationToken);

                    return result.Match(
                        Results.Ok,
                        ResultsHelper.Problem);
                })
            .Produces<ReportActivitiesPerformanceResponse>()
            .HasApiVersion(1.0)
            .WithName(nameof(ApiEndpoints.Reports.GetReportActivitiesPerformance));
        
        return builder;
    }
}