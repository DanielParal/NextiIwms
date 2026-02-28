using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Mmo.Settings.Application.SpecialInformations.Commands.DeleteSpecialInformation;

namespace Nexticz.Module.Mmo.Settings.Presentation.SpecialInformations;

internal static class DeleteSpecialInformationEndpoint
{
    public static IEndpointRouteBuilder MapDeleteSpecialInformationEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapDelete(SettingsEndpoints.SpecialInformationEndpoints.DeleteSpecialInformation,
                async (
                    Guid id,  
                    ISender sender,
                    CancellationToken cancellationToken) =>
                {
                    var result = await sender.Send(new DeleteSpecialInformationCommand(id), cancellationToken);

                    return result.Match(
                        _ => Results.NoContent(),
                        ResultsHelper.Problem);
                })
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.SpecialInformationEndpoints.DeleteSpecialInformation)));

        return builder;
    }
}