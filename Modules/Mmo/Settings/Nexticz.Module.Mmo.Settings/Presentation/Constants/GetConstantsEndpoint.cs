using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Mmo.Settings.Contracts.Constants;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Lib.Shared.Extensions;
using Nexticz.Module.Mmo.Settings.Application.Constants;
using Nexticz.Module.Mmo.Settings.Application.Constants.Queries.GetConstants;
using Nexticz.Module.Mmo.Settings.Application.Grouping;
using Nexticz.Module.Mmo.Settings.Domain.ConstantEntity;


namespace Nexticz.Module.Mmo.Settings.Presentation.Constants;

internal static class GetConstantsEndpoint
{
    public static IEndpointRouteBuilder MapGetConstants(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(SettingsEndpoints.ConstantEndpoints.GetConstants,
                async (
                    [AsParameters] BaseFilteringParams filteringParams,
                    ISender mediator,
                    CancellationToken cancellationToken,
                    [FromServices] ISettingsGroupedResultHandler groupedResultHandler
                ) =>
                {
                    if (filteringParams.Group is not null)
                    {
                        return Results.Ok(await groupedResultHandler.GetGroupedResultAsync<Constant>(filteringParams, cancellationToken));
                    }
                    
                    var result = await mediator.Send(new GetConstantsQuery(filteringParams), cancellationToken);
                    
                    return Results.Ok(result.MapDataFromTInToTOut(ConstantResponseFactory.Create));
                })
            .Produces<FilteredResult<ConstantResponse>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.ConstantEndpoints.GetConstants)));

        return builder;
    }
}