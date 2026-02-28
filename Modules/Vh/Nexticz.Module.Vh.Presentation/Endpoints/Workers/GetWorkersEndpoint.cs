using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Vh.Application.Workers.Common.Models;
using Nexticz.Module.Vh.Application.Workers.Queries.GetWorkers;
using Nexticz.Module.Vh.Contracts.Workers;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Lib.Shared.Helpers;


namespace Nexticz.Module.Vh.Presentation.Endpoints.Workers;

public static class GetWorkersEndpoint
{
    public static IEndpointRouteBuilder MapGetWorkers(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(ApiEndpoints.Workers.GetWorkers,
                async ([AsParameters] WorkersFilteringParams filteringParams, ISender mediatr,
                    CancellationToken cancellationToken) =>
                {
                    var query = new GetWorkersQuery { FilteringParams = filteringParams };
                    var result = await mediatr.Send(query, cancellationToken);

                    return result.Match(
                        Results.Ok,
                        ResultsHelper.Problem);
                })
            .Produces<FilteredResult<WorkerResponse>>()
            .HasApiVersion(1.0)
            .WithName(nameof(ApiEndpoints.Workers.GetWorkers));

        return builder;
    }
}