using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Mmo.Settings.Contracts.Kits;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Mmo.Settings.Application.Kits.Commands.UpdateKit;


namespace Nexticz.Module.Mmo.Settings.Presentation.Kits;

internal static class UpdateKitEndpoint
{
    public static IEndpointRouteBuilder MapUpdateKit(this IEndpointRouteBuilder builder)
    {
        builder.MapPut(SettingsEndpoints.KitEndpoints.UpdateKit,
                async (
                    string code, 
                    UpdateKitRequest request, 
                    ISender mediatr,
                    CancellationToken cancellationToken) =>
                {
                    var command = new UpdateKitCommand(code, request);
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
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.KitEndpoints.UpdateKit)));
        
        return builder;
    }
}