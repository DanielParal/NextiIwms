using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Sign.Settings.Contracts.DepositorGroups;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Lib.Shared.Extensions;
using Nexticz.Module.Sign.Settings.Application.DepositorGroups.Queries.GetDepositorGroups;
using Nexticz.Module.Sign.Settings.Application.Grouping;
using Nexticz.Module.Sign.Settings.Domain.DepositorGroupAggregate;


namespace Nexticz.Module.Sign.Settings.Presentation.DepositorGroups;

internal static class GetDepositorGroupsEndpoint
{
    public static IEndpointRouteBuilder MapGetDepositorGroupsEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(SettingsEndpoints.DepositorGroupEndpoints.GetDepositorGroups,
                async (
                    [AsParameters] BaseFilteringParams filteringParams,
                    ISender sender,
                    CancellationToken cancellationToken,
                    [FromServices] ISettingsGroupedResultHandler groupedResultHandler) =>
                {
                    if (filteringParams.Group is not null)
                    {
                        return Results.Ok(await groupedResultHandler.GetGroupedResultAsync<DepositorGroup>(filteringParams, cancellationToken));
                    }
                    
                    var result = await sender.Send(new GetDepositorGroupsQuery(filteringParams), cancellationToken);
                    
                    return Results.Ok(result.MapDataFromTInToTOut(DepositorGroupResponseFactory.Create));
                })
            .Produces<FilteredResult<DepositorGroupResponse>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.DepositorGroupEndpoints.GetDepositorGroups)));

        return builder;
    }
}