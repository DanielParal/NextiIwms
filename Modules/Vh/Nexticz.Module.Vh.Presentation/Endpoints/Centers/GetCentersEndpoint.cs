using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Vh.Application.Centers.Common.Models;
using Nexticz.Module.Vh.Application.Centers.Queries.ListCenters;
using Nexticz.Module.Vh.Contracts.Centers;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Lib.Shared.Helpers;


namespace Nexticz.Module.Vh.Presentation.Endpoints.Centers;

public static class GetCentersEndpoint
{
    public static IEndpointRouteBuilder MapGetCenters(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(ApiEndpoints.Centers.GetCenters, 
            async ([AsParameters] CentersFilteringParams filteringParams,
                ISender mediatr, CancellationToken cancellationToken) =>
            {
                var query = new ListCentersQuery { FilteringParams = filteringParams };
                var result = await mediatr.Send(query, cancellationToken);

                return result.Match(
                    Results.Ok,
                    ResultsHelper.Problem);
            })
            .Produces<FilteredResult<CenterResponse>>()
            .HasApiVersion(1.0)
            .WithName(nameof(ApiEndpoints.Centers.GetCenters));
        
        return builder;
    }
}