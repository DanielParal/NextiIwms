using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Mmo.Settings.Contracts.KitSapDefinitions;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Lib.Shared.Extensions;
using Nexticz.Module.Mmo.Settings.Application.Grouping;
using Nexticz.Module.Mmo.Settings.Application.KitSapDefinitions;
using Nexticz.Module.Mmo.Settings.Application.KitSapDefinitions.Queries.GetKitSapDefinitions;
using Nexticz.Module.Mmo.Settings.Domain.KitSapDefinitionEntity;


namespace Nexticz.Module.Mmo.Settings.Presentation.KitSapDefinitions;

internal static class GetKitSapDefinitionsEndpoint
{
    public static IEndpointRouteBuilder MapGetKitSapDefinitions(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(SettingsEndpoints.KitSapDefinitionEndpoints.GetKitSapDefinitions,
                async (
                    [AsParameters] BaseFilteringParams filteringParams,
                    ISender mediator,
                    CancellationToken cancellationToken,
                    [FromServices] ISettingsGroupedResultHandler groupedResultHandler) =>
                {
                    if (filteringParams.Group is not null)
                    {
                        return Results.Ok(await groupedResultHandler.GetGroupedResultAsync<KitSapDefinition>(filteringParams, cancellationToken));
                    }
                    
                    var query = new GetKitSapDefinitionsQuery(filteringParams);
                    var result = await mediator.Send(query, cancellationToken);
                    
                    return Results.Ok(result.MapDataFromTInToTOut(KitSapDefinitionResponseFactory.Create));
                })
            .Produces<FilteredResult<KitSapDefinitionResponse>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.KitSapDefinitionEndpoints.GetKitSapDefinitions)));

        return builder;
    }
}