using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Mmo.Settings.Contracts.SpecialInformations;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Mmo.Settings.Application.SpecialInformations.Commands.UpdateSpecialInformation;


namespace Nexticz.Module.Mmo.Settings.Presentation.SpecialInformations;

internal static class UpdateSpecialInformationEndpoint
{
    public static IEndpointRouteBuilder MapUpdateSpecialInformationEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapPut(SettingsEndpoints.SpecialInformationEndpoints.UpdateSpecialInformation,
                async (
                    Guid id, 
                    UpdateSpecialInformationRequest request, 
                    ISender sender,
                    CancellationToken cancellationToken) =>
                {
                    var result = await sender.Send(
                        new UpdateSpecialInformationCommand(id, request.Title, request.Description), cancellationToken);

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
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.SpecialInformationEndpoints.UpdateSpecialInformation)));
        
        return builder;
    }
}