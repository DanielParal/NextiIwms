using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Mmo.Settings.Contracts.PackagingTypes;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Lib.Shared.Extensions;
using Nexticz.Module.Mmo.Settings.Application.Grouping;
using Nexticz.Module.Mmo.Settings.Application.PackagingTypes.Queries.GetPackagingTypes;
using Nexticz.Module.Mmo.Settings.Domain.PackagingTypeEntity;


namespace Nexticz.Module.Mmo.Settings.Presentation.PackagingTypes;

internal static class GetPackagingTypesEndpoint
{
    public static IEndpointRouteBuilder MapGetPackagingTypes(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(SettingsEndpoints.PackagingTypeEndpoints.GetPackagingTypes,
                async (
                    [AsParameters] BaseFilteringParams filteringParams,
                    ISender mediator,
                    CancellationToken cancellationToken,
                    [FromServices] ISettingsGroupedResultHandler groupedResultHandler) =>
                {
                    if (filteringParams.Group is not null)
                    {
                        return Results.Ok(await groupedResultHandler.GetGroupedResultAsync<PackagingType>(filteringParams, cancellationToken));
                    }
                    
                    var query = new GetPackagingTypesQuery(filteringParams);
                    var result = await mediator.Send(query, cancellationToken);
                    
                    return Results.Ok(result.MapDataFromTInToTOut(PackagingTypeResponseFactory.Create));
                })
            .Produces<FilteredResult<PackagingTypeResponse>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.PackagingTypeEndpoints.GetPackagingTypes)));

        return builder;
    }
}