using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Portal.Contracts.Modules;
using Nexticz.Module.Portal.Domain.ModuleAggregate;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Portal.Application.Grouping;
using Nexticz.Module.Portal.Application.Modules.Queries.GetModules;

namespace Nexticz.Module.Portal.Presentation.Modules;

internal static class GetModulesEndpoint
{
    public static IEndpointRouteBuilder MapGetModulesEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(PortalEndpoints.ModuleEndpoints.GetModules,
                async (
                    [AsParameters] BaseFilteringParams filteringParams,
                    ISender sender,
                    CancellationToken cancellationToken,
                    [FromServices] IPortalGroupedResultHandler groupedResultHandler) =>
                {
                    if (filteringParams.Group is not null)
                    {
                        return Results.Ok(await groupedResultHandler.GetGroupedResultAsync<Domain.ModuleAggregate.Module>(filteringParams, cancellationToken));
                    }
                    
                    var result = await sender.Send(new GetModulesQuery(filteringParams), cancellationToken);
                    
                    return Results.Ok(result.MapDataFromTInToTOut(ModuleResponseFactory.Create));
                })
            .Produces<FilteredResult<ModuleResponse>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .HasApiVersion(1.0)
            .WithName(PortalEndpoints.GetOpenApiName(nameof(PortalEndpoints.ModuleEndpoints.GetModules)));

        return builder;
    }
}