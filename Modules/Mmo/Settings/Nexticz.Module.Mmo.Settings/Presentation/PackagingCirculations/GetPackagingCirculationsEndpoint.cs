using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Mmo.Settings.Contracts.PackagingCirculations;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Lib.Shared.Extensions;
using Nexticz.Module.Mmo.Settings.Application.Grouping;
using Nexticz.Module.Mmo.Settings.Application.PackagingCirculations.Queries.GetPackagingCirculations;
using Nexticz.Module.Mmo.Settings.Domain.PackagingCirculationEntity;


namespace Nexticz.Module.Mmo.Settings.Presentation.PackagingCirculations;

internal static class GetPackagingCirculationsEndpoint
{
    public static IEndpointRouteBuilder MapGetPackagingCirculations(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(SettingsEndpoints.PackagingCirculationEndpoints.GetPackagingCirculations,
                async (
                    [AsParameters] BaseFilteringParams filteringParams,
                    ISender mediator,
                    CancellationToken cancellationToken,
                    [FromServices] ISettingsGroupedResultHandler groupedResultHandler
                ) =>
                {
                    if (filteringParams.Group is not null)
                    {
                        return Results.Ok(await groupedResultHandler.GetGroupedResultAsync<PackagingCirculation>(filteringParams, cancellationToken));
                    }
                    
                    var request = new GetPackagingCirculationsQuery(filteringParams);
                    var result = await mediator.Send(request, cancellationToken);
                    
                    return Results.Ok(result.MapDataFromTInToTOut(PackagingCirculationResponseFactory.Create));
                })
            .Produces<FilteredResult<PackagingCirculationResponse>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.PackagingCirculationEndpoints.GetPackagingCirculations)));
        
        return builder;
    }
}