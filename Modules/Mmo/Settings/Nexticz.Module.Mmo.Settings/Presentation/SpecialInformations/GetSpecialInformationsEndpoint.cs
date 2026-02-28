using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Mmo.Settings.Contracts.SpecialInformations;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Lib.Shared.Extensions;
using Nexticz.Module.Mmo.Settings.Application.Grouping;
using Nexticz.Module.Mmo.Settings.Application.SpecialInformations;
using Nexticz.Module.Mmo.Settings.Application.SpecialInformations.Queries.GetSpecialInformations;
using Nexticz.Module.Mmo.Settings.Domain.SpecialInformationEntity;


namespace Nexticz.Module.Mmo.Settings.Presentation.SpecialInformations;

internal static class GetSpecialInformationsEndpoint
{
    public static IEndpointRouteBuilder MapGetSpecialInformationsEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(SettingsEndpoints.SpecialInformationEndpoints.GetSpecialInformations,
                async (
                    [AsParameters] BaseFilteringParams filteringParams,
                    ISender sender,
                    CancellationToken cancellationToken,
                    [FromServices] ISettingsGroupedResultHandler groupedResultHandler) =>
                {
                    if (filteringParams.Group is not null)
                    {
                        return Results.Ok(await groupedResultHandler.GetGroupedResultAsync<SpecialInformation>(filteringParams, cancellationToken));
                    }
                    
                    var result = await sender.Send(new GetSpecialInformationsQuery(filteringParams), cancellationToken);
                    
                    return Results.Ok(
                        result.MapDataFromTInToTOut(
                            SpecialInformationResponseFactory.Create));
                })
            .Produces<FilteredResult<SpecialInformationResponse>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.SpecialInformationEndpoints.GetSpecialInformations)));

        return builder;
    }
}