using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Sign.Settings.Contracts.Printers;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Sign.Settings.Application.Printers.Commands.CreatePrinter;

namespace Nexticz.Module.Sign.Settings.Presentation.Printers;

internal static class CreatePrinterEndpoint
{
    public static IEndpointRouteBuilder MapCreatePrinterEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapPost(SettingsEndpoints.PrinterEndpoints.CreatePrinter,
                async (
                    CreatePrinterRequest request,
                    ISender sender,
                    CancellationToken cancellationToken) =>
                {
                    var result = 
                        await sender.Send(new CreatePrinterCommand(request.Code, request.Name, request.Ip), cancellationToken);

                    return result.Match(
                        partner => Results.Created(
                            $"{SettingsEndpoints.PrinterEndpoints.GetPrinters}/{partner.Code}",
                            PrinterResponseFactory.Create(partner)),
                        ResultsHelper.Problem);
                })
            .Produces<PrinterResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces<ApiErrorResponse>(StatusCodes.Status422UnprocessableEntity)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.PrinterEndpoints.CreatePrinter)));

        return builder;
    }
}