using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Mmo.Settings.Contracts.Constants;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Mmo.Settings.Application.Constants.Commands.UpdateConstant;


namespace Nexticz.Module.Mmo.Settings.Presentation.Constants;

internal static class UpdateConstantEndpoint
{
    public static IEndpointRouteBuilder MapUpdateConstant(this IEndpointRouteBuilder builder)
    {
        builder.MapPut(SettingsEndpoints.ConstantEndpoints.UpdateConstant,
                async (
                    string key,
                    UpdateConstantRequest request, 
                    ISender mediator, 
                    CancellationToken cancellationToken) =>
                {
                    var command = new UpdateConstantCommand(key, request.Value, request.Description);
                    
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
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.ConstantEndpoints.UpdateConstant)));

        return builder;
    }
}