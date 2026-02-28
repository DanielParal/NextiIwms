using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Mmo.Settings.Contracts.Kits;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Mmo.Settings.Application.Kits;
using Nexticz.Module.Mmo.Settings.Application.Kits.Commands.CreateKit;

namespace Nexticz.Module.Mmo.Settings.Presentation.Kits;

internal static class CreateKitEndpoint
{
    public static IEndpointRouteBuilder MapCreateKit(this IEndpointRouteBuilder builder)
    {
        builder.MapPost(SettingsEndpoints.KitEndpoints.CreateKit,
                async (
                    CreateKitRequest request, 
                    ISender sender, 
                    [FromServices] KitResponseFactory kitResponseFactory,
                    CancellationToken cancellationToken) =>
                {
                    var command = new CreateKitCommand(request);
                    var result = await sender.Send(command, cancellationToken);

                    if (result.IsError)
                    {
                        return ResultsHelper.Problem(result.Errors);
                    }
                    
                    var kitResponse = await kitResponseFactory.CreateAsync(result.Value, cancellationToken);
                    
                    return Results.Created($"{SettingsEndpoints.KitEndpoints.GetKits}/{kitResponse.Code}", kitResponse);
                })
            .Produces<KitResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces<ApiErrorResponse>(StatusCodes.Status422UnprocessableEntity)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.KitEndpoints.CreateKit)));

        return builder;
    }
}