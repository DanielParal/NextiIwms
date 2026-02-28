using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Mmo.Settings.Contracts.InactivityTypes;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Lib.Shared.Extensions;
using Nexticz.Module.Mmo.Settings.Application.Grouping;
using Nexticz.Module.Mmo.Settings.Application.InactivityTypes;
using Nexticz.Module.Mmo.Settings.Application.InactivityTypes.Queries.GetInactivityTypes;
using Nexticz.Module.Mmo.Settings.Domain.InactivityTypeEntity;


namespace Nexticz.Module.Mmo.Settings.Presentation.InactivityTypes;

internal static class GetInactivityTypesEndpoint
{
    public static IEndpointRouteBuilder MapGetInactivityTypesEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(SettingsEndpoints.InactivityTypeEndpoints.GetInactivityTypes,
                async (
                    [AsParameters] BaseFilteringParams filteringParams,
                    ISender sender,
                    CancellationToken cancellationToken,
                    [FromServices] ISettingsGroupedResultHandler groupedResultHandler
                ) =>
                {
                    if (filteringParams.Group is not null)
                    {
                        return Results.Ok(await groupedResultHandler.GetGroupedResultAsync<InactivityType>(filteringParams, cancellationToken));
                    }
                    
                    var result = await sender.Send(new GetInactivityTypesQuery(filteringParams), cancellationToken);
                    
                    return Results.Ok(result.MapDataFromTInToTOut(InactivityTypeResponseFactory.Create));
                })
            .Produces<FilteredResult<InactivityTypeResponse>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.InactivityTypeEndpoints.GetInactivityTypes)));

        return builder;
    }
}