using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Mmo.Settings.Contracts.Manufactures;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Mmo.Settings.Application.Manufactures.Commands.UpdateManufacture;


namespace Nexticz.Module.Mmo.Settings.Presentation.Manufactures;

internal static class UpdateManufactureEndpoint
{
    public static IEndpointRouteBuilder MapUpdateManufacture(this IEndpointRouteBuilder builder)
    {
        builder.MapPut(SettingsEndpoints.ManufactureEndpoints.UpdateManufacture,
                async (
                    string code,
                    UpdateManufactureRequest request, 
                    ISender mediator, 
                    CancellationToken cancellationToken) =>
                {
                    var command = new UpdateManufactureCommand(code, request.Name);
                    
                    var result = await mediator.Send(command, cancellationToken);

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
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.ManufactureEndpoints.UpdateManufacture)));

        return builder;
    }
}