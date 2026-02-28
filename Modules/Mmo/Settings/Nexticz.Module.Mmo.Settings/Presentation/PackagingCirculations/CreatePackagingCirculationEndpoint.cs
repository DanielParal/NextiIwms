using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Mmo.Settings.Contracts.PackagingCirculations;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Mmo.Settings.Application.PackagingCirculations.Commands.CreatePackagingCirculation;


namespace Nexticz.Module.Mmo.Settings.Presentation.PackagingCirculations;

internal static class CreatePackagingCirculationEndpoint
{
    public static IEndpointRouteBuilder MapCreatePackagingCirculation(this IEndpointRouteBuilder builder)
    {
        builder.MapPost(SettingsEndpoints.PackagingCirculationEndpoints.CreatePackagingCirculation,
                async (
                    CreatePackagingCirculationRequest request,
                    ISender mediator,
                    CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new CreatePackagingCirculationCommand(request.Code, request.Name), cancellationToken);
        
                    return result.Match(
                        packagingCirculation => 
                            Results.Created($"/{SettingsEndpoints.PackagingCirculationEndpoints.GetPackagingCirculations}/{packagingCirculation.Code}", 
                                PackagingCirculationResponseFactory.Create(packagingCirculation)),
                        ResultsHelper.Problem);
                })
            .Produces<PackagingCirculationResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces<ApiErrorResponse>(StatusCodes.Status422UnprocessableEntity)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.PackagingCirculationEndpoints.CreatePackagingCirculation)));

        return builder;
    }
}