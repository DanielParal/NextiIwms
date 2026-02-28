using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Mmo.Settings.Contracts.Packagings;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Lib.Shared.Extensions;
using Nexticz.Module.Mmo.Settings.Application.Grouping;
using Nexticz.Module.Mmo.Settings.Application.Packagings;
using Nexticz.Module.Mmo.Settings.Application.Packagings.Queries.GetPackagings;
using Nexticz.Module.Mmo.Settings.Domain.PackagingAggregate;


namespace Nexticz.Module.Mmo.Settings.Presentation.Packagings;

internal static class GetPackagingsEndpoint
{
    public static IEndpointRouteBuilder MapGetPackagings(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(SettingsEndpoints.PackagingEndpoints.GetPackagings,
                async (
                    [AsParameters] BaseFilteringParams filteringParams,
                    ISender sender,
                    CancellationToken cancellationToken,
                    [FromServices] ISettingsGroupedResultHandler groupedResultHandler) =>
                {
                    if (filteringParams.Group is not null)
                    {
                        return Results.Ok(await groupedResultHandler.GetGroupedResultAsync<Packaging>(filteringParams, cancellationToken));
                    }

                    var result = await sender.Send(new GetPackagingsQuery(filteringParams), cancellationToken);
                    
                    return Results.Ok(
                        result.MapDataFromTInToTOut(
                            PackagingResponseFactory.Create));
                })
            .Produces<FilteredResult<PackagingResponse>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.PackagingEndpoints.GetPackagings)));

        return builder;
    }
}