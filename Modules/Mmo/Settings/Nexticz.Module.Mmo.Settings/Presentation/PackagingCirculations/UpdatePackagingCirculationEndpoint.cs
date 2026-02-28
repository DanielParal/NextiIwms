using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Mmo.Settings.Contracts.PackagingCirculations;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Mmo.Settings.Application.PackagingCirculations.Commands.UpdatePackagingCirculation;


namespace Nexticz.Module.Mmo.Settings.Presentation.PackagingCirculations;

internal static class UpdatePackagingCirculationEndpoint
{
    public static IEndpointRouteBuilder MapUpdatePackagingCirculation(this IEndpointRouteBuilder builder)
    {
        builder.MapPut(SettingsEndpoints.PackagingCirculationEndpoints.UpdatePackagingCirculation,
                async (
                    string code,
                    UpdatePackagingCirculationRequest request, 
                    ISender mediator, 
                    CancellationToken cancellationToken) =>
                {
                    var command = new UpdatePackagingCirculationCommand(code, request.Name);
                    
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
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.PackagingCirculationEndpoints.UpdatePackagingCirculation)));

        return builder;
    }
}