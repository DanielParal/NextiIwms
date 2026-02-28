using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Mmo.Settings.Contracts.Manufactures;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Mmo.Settings.Application.Manufactures.Commands.CreateManufacture;


namespace Nexticz.Module.Mmo.Settings.Presentation.Manufactures;

internal static class CreateManufactureEndpoint
{
    public static IEndpointRouteBuilder MapCreateManufacture(this IEndpointRouteBuilder builder)
    {
        builder.MapPost(SettingsEndpoints.ManufactureEndpoints.CreateManufacture,
                async (
                    CreateManufactureRequest request,
                    ISender mediator,
                    CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new CreateManufactureCommand(request.Code, request.Name), cancellationToken);
        
                    return result.Match(
                        manufacture => 
                            Results.Created($"/{SettingsEndpoints.ManufactureEndpoints.GetManufactures}/{manufacture.Code}", 
                            ManufactureResponseFactory.Create(manufacture)),
                        ResultsHelper.Problem);
                })
            .Produces<ManufactureResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces<ApiErrorResponse>(StatusCodes.Status422UnprocessableEntity)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.ManufactureEndpoints.CreateManufacture)));

        return builder;
    }
}