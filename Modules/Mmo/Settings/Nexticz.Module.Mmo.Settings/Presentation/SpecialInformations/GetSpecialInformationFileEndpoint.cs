using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.FileHandling.Models;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Mmo.Settings.Application.SpecialInformations.Queries.GetSpecialInformationFile;


namespace Nexticz.Module.Mmo.Settings.Presentation.SpecialInformations;

internal static class GetSpecialInformationFileEndpoint
{
    public static IEndpointRouteBuilder MapGetSpecialInformationFileEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(SettingsEndpoints.SpecialInformationEndpoints.GetFile,
                async (
                    Guid id,
                    ISender sender,
                    CancellationToken cancellationToken
                ) =>
                {
                    var fileResult = 
                        await sender.Send(new GetSpecialInformationFileQuery(id), cancellationToken);
                    
                    return fileResult.Match(
                        file => 
                            Results.Ok(new FileResponse(file.ContentBytes, file.ContentType, file.FileName)),
                        ResultsHelper.Problem);
                })
            .Produces<FileResponse>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces<ApiErrorResponse>(StatusCodes.Status422UnprocessableEntity)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.SpecialInformationEndpoints.GetFile)));

        return builder;
    }
}