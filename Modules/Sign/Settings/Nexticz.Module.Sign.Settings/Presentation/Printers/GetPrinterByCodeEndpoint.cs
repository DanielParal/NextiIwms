using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Sign.Settings.Contracts.Printers;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Sign.Settings.Application.Printers.Queries.GetPrinterByCode;

namespace Nexticz.Module.Sign.Settings.Presentation.Printers;

internal static class GetPrinterByCodeEndpoint
{
    public static IEndpointRouteBuilder MapGetPrinterByCodeEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(SettingsEndpoints.PrinterEndpoints.GetPrinterByCode,
                async (
                    string code, 
                    ISender sender,
                    CancellationToken cancellationToken
                ) =>
                {
                    var result = 
                        await sender.Send(new GetPrinterByCodeQuery(code), cancellationToken);
                    
                    return result.Match(
                        depositor => Results.Ok(PrinterResponseFactory.Create(depositor)),
                        ResultsHelper.Problem);
                })
            .Produces<PrinterResponse>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.PrinterEndpoints.GetPrinterByCode)));

        return builder;
    }
}