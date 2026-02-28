using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Mmo.Settings.Contracts.WashingMachines;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Lib.Shared.Extensions;
using Nexticz.Module.Mmo.Settings.Application.Grouping;
using Nexticz.Module.Mmo.Settings.Application.WashingMachines;
using Nexticz.Module.Mmo.Settings.Application.WashingMachines.Queries.GetWashingMachines;
using Nexticz.Module.Mmo.Settings.Domain.WashingMachineEntity;


namespace Nexticz.Module.Mmo.Settings.Presentation.WashingMachines;

internal static class GetWashingMachinesEndpoint
{
    public static IEndpointRouteBuilder MapGetWashingMachines(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(SettingsEndpoints.WashingMachineEndpoints.GetWashingMachines,
                async (
                    [AsParameters] BaseFilteringParams filteringParams,
                    ISender mediator,
                    CancellationToken cancellationToken,
                    [FromServices] ISettingsGroupedResultHandler groupedResultHandler) =>
                {
                    if (filteringParams.Group is not null)
                    {
                        return Results.Ok(await groupedResultHandler.GetGroupedResultAsync<WashingMachine>(filteringParams, cancellationToken));
                    }
                    
                    var query = new GetWashingMachinesQuery(filteringParams);
                    var result = await mediator.Send(query, cancellationToken);
                    
                    return Results.Ok(result.MapDataFromTInToTOut(WashingMachineResponseFactory.Create));
                })
            .Produces<FilteredResult<WashingMachineResponse>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.WashingMachineEndpoints.GetWashingMachines)));

        return builder;
    }
}