
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Mmo.Settings.Contracts.Depositors;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Lib.Shared.Extensions;
using Nexticz.Module.Mmo.Settings.Application.Depositors;
using Nexticz.Module.Mmo.Settings.Application.Depositors.Queries.GetDepositors;
using Nexticz.Module.Mmo.Settings.Application.Grouping;
using Nexticz.Module.Mmo.Settings.Domain.DepositorEntity;


namespace Nexticz.Module.Mmo.Settings.Presentation.Depositors;

internal static class GetDepositorsEndpoint
{
    public static IEndpointRouteBuilder MapGetDepositors(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(SettingsEndpoints.DepositorEndpoints.GetDepositors,
                async (
                    [AsParameters] BaseFilteringParams filteringParams,
                    ISender mediator,
                    CancellationToken cancellationToken,
                    [FromServices] ISettingsGroupedResultHandler groupedResultHandler) =>
                {
                    if (filteringParams.Group is not null)
                    {
                        return Results.Ok(await groupedResultHandler.GetGroupedResultAsync<Depositor>(filteringParams, cancellationToken));
                    }
                    
                    var query = new GetDepositorsQuery(filteringParams);
                    var result = await mediator.Send(query, cancellationToken);
                    
                    return Results.Ok(result.MapDataFromTInToTOut(DepositorResponseFactory.Create));
                })
            .Produces<FilteredResult<DepositorResponse>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.DepositorEndpoints.GetDepositors)));

        return builder;
    }
}