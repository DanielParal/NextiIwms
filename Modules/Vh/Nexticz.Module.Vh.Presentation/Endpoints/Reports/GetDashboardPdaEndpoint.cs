using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Vh.Application.Reports.Queries.GetDashboardPda;
using Nexticz.Module.Vh.Contracts.Reports;
using Nexticz.Lib.Shared.Helpers;

namespace Nexticz.Module.Vh.Presentation.Endpoints.Reports;

public static class GetDashboardPdaEndpoint
{
    public static IEndpointRouteBuilder MapDashboardPda(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(ApiEndpoints.Reports.GetDashboardPda,
                async ([AsParameters] DashboardPdaRequest filteringParams,
                    ISender mediatr, CancellationToken cancellationToken) =>
                {
                    var query = new GetDashboardPdaQuery(filteringParams);
                    var result = await mediatr.Send(query, cancellationToken);

                    return result.Match(
                        Results.Ok, ResultsHelper.Problem);
                })
            .Produces<DashboardPdaResponse>()
            .HasApiVersion(1.0)
            .WithName(nameof(ApiEndpoints.Reports.GetDashboardPda))
            .AllowAnonymous();

        return builder;
    }
}