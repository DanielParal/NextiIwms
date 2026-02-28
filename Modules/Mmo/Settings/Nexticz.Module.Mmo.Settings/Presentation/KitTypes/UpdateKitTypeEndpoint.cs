using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Mmo.Settings.Contracts.KitTypes;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Mmo.Settings.Application.KitTypes.Commands.UpdateKitType;


namespace Nexticz.Module.Mmo.Settings.Presentation.KitTypes;

internal static class UpdateKitTypeEndpoint
{
    public static IEndpointRouteBuilder MapUpdateKitType(this IEndpointRouteBuilder builder)
    {
        builder.MapPut(SettingsEndpoints.KitTypeEndpoints.UpdateKitType,
                async (
                    string code, 
                    UpdateKitTypeRequest request, 
                    ISender mediatr,
                    CancellationToken cancellationToken) =>
                {
                    var command = new UpdateKitTypeCommand(code, request.Name);
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
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.KitTypeEndpoints.UpdateKitType)));
        
        return builder;
    }
}