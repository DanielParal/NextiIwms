using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Mmo.Settings.Contracts.Depositors;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Mmo.Settings.Application.Depositors.Commands.UpdateDepositor;


namespace Nexticz.Module.Mmo.Settings.Presentation.Depositors;

internal static class UpdateDepositorEndpoint
{
    public static IEndpointRouteBuilder MapUpdateDepositor(this IEndpointRouteBuilder builder)
    {
        builder.MapPut(SettingsEndpoints.DepositorEndpoints.UpdateDepositor,
                async (
                    string code, 
                    UpdateDepositorRequest request, 
                    ISender mediatr,
                    CancellationToken cancellationToken) =>
                {
                    var command = new UpdateDepositorCommand(code, request.Name, request.BarcodeTemplate);
                    var result = await mediatr.Send(command, cancellationToken);

                    return result.Match(
                        _ => Results.NoContent(),
                        ResultsHelper.Problem);
                })
            .Produces<object>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound)
            .Produces<ApiErrorResponse>(StatusCodes.Status422UnprocessableEntity)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.DepositorEndpoints.UpdateDepositor)));
        
        return builder;
    }
}