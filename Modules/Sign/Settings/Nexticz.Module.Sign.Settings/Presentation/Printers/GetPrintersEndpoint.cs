using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Sign.Settings.Contracts.Printers;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Sign.Settings.Application.Grouping;
using Nexticz.Module.Sign.Settings.Application.Printers.Queries.GetPrinters;
using Nexticz.Module.Sign.Settings.Domain.PrinterAggregate;

namespace Nexticz.Module.Sign.Settings.Presentation.Printers;

internal static class GetPrintersEndpoint
{
    public static IEndpointRouteBuilder MapGetPrintersEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(SettingsEndpoints.PrinterEndpoints.GetPrinters,
                async (
                    [AsParameters] BaseFilteringParams filteringParams,
                    ISender sender,
                    CancellationToken cancellationToken,
                    [FromServices] ISettingsGroupedResultHandler groupedResultHandler) =>
                {
                    if (filteringParams.Group is not null)
                    {
                        return Results.Ok(await groupedResultHandler.GetGroupedResultAsync<Printer>(filteringParams, cancellationToken));
                    }
                    
                    var result = await sender.Send(new GetPrintersQuery(filteringParams), cancellationToken);
                    
                    return Results.Ok(result.MapDataFromTInToTOut(PrinterResponseFactory.Create));
                })
            .Produces<FilteredResult<PrinterResponse>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.PrinterEndpoints.GetPrinters)));

        return builder;
    }
}