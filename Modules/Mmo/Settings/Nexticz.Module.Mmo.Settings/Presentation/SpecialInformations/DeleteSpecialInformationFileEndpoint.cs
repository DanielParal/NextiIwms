using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Mmo.Settings.Application.SpecialInformations.Commands.DeleteSpecialInformationFile;

namespace Nexticz.Module.Mmo.Settings.Presentation.SpecialInformations;

internal static class DeleteSpecialInformationFileEndpoint
{
    public static IEndpointRouteBuilder MapDeleteSpecialInformationFileEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapDelete(SettingsEndpoints.SpecialInformationEndpoints.DeleteFile,
                async (
                    Guid id,  
                    ISender sender,
                    CancellationToken cancellationToken) =>
                {
                    var result = await sender.Send(new DeleteSpecialInformationFileCommand(id), cancellationToken);

                    return result.Match(
                        _ => Results.NoContent(),
                        ResultsHelper.Problem);
                })
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.SpecialInformationEndpoints.DeleteFile)));

        return builder;
    }
}