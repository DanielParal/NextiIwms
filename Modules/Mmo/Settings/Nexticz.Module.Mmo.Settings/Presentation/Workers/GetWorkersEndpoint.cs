using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Mmo.Settings.Contracts.Workers;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Lib.Shared.Extensions;
using Nexticz.Module.Mmo.Settings.Application.Grouping;
using Nexticz.Module.Mmo.Settings.Application.Workers;
using Nexticz.Module.Mmo.Settings.Application.Workers.Queries.GetWorkers;
using Nexticz.Module.Mmo.Settings.Domain.WorkerEntity;


namespace Nexticz.Module.Mmo.Settings.Presentation.Workers;

internal static class GetWorkersEndpoint
{
    public static IEndpointRouteBuilder MapGetWorkers(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(SettingsEndpoints.WorkerEndpoints.GetWorkers,
                async (
                    [AsParameters] BaseFilteringParams filteringParams,
                    ISender mediator,
                    CancellationToken cancellationToken,
                    [FromServices] ISettingsGroupedResultHandler groupedResultHandler
                ) =>
                {
                    if (filteringParams.Group is not null)
                    {
                        return Results.Ok(await groupedResultHandler.GetGroupedResultAsync<Worker>(filteringParams, cancellationToken));
                    }
                    
                    var result = await mediator.Send(new GetWorkersQuery(filteringParams), cancellationToken);
                    
                    return Results.Ok(result.MapDataFromTInToTOut(WorkerResponseFactory.Create));
                })
            .Produces<FilteredResult<WorkerResponse>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.WorkerEndpoints.GetWorkers)));

        return builder;
    }
}