using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.FileHandling.Models;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Mmo.Settings.Application.Kits.Queries.GetKitInstructionFile;


namespace Nexticz.Module.Mmo.Settings.Presentation.Kits;

internal static class GetKitInstructionFileEndpoint
{
    public static IEndpointRouteBuilder MapGetKitInstructionFileEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(SettingsEndpoints.KitEndpoints.GetKitInstructionFile,
                async (
                    string code,
                    ISender sender,
                    CancellationToken cancellationToken
                ) =>
                {
                    var kitInstructionResult = 
                        await sender.Send(new GetKitInstructionFileQuery(code), cancellationToken);
                    
                    return kitInstructionResult.Match(
                        instructionFile => 
                            Results.Ok(new FileResponse(instructionFile.ContentBytes, instructionFile.ContentType, instructionFile.FileName)),
                        ResultsHelper.Problem);
                })
            .Produces<FileResponse>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces<ApiErrorResponse>(StatusCodes.Status422UnprocessableEntity)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.KitEndpoints.GetKitInstructionFile)));

        return builder;
    }
}