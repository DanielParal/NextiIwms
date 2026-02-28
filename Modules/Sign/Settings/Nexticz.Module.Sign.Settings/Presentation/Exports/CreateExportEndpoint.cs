using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Sign.Settings.Contracts.Exports;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.FileHandling.Models;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Lib.Shared.UserProviders;
using Nexticz.Module.Sign.Settings.Application.ImportsExports.Exports.Commands.CreateExport;
using Nexticz.Module.Sign.Settings.Domain.ExportAggregate;

namespace Nexticz.Module.Sign.Settings.Presentation.Exports;

internal static class CreateExportsEndpoint
{
    public static IEndpointRouteBuilder MapCreateExport(this IEndpointRouteBuilder builder)
    {
        builder.MapPost(SettingsEndpoints.ExportEndpoints.CreateExport,
                async (
                    CreateExportRequest request,
                    ISender sender,
                    CancellationToken cancellationToken,
                    [FromServices] ICurrentUserProvider currentUserProvider) =>
                {
                    var result = await sender.Send(new CreateExportCommand(currentUserProvider.GetCurrentUser().UserName, (ExportType)request.ExportType), cancellationToken);
                    
                    return result.Match(
                        file => 
                            Results.Ok(new FileResponse(file.ContentBytes, file.ContentType, file.FileName)),
                        ResultsHelper.Problem);
                })
            .Produces<FileResponse>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces<ApiErrorResponse>(StatusCodes.Status422UnprocessableEntity)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.ExportEndpoints.CreateExport)));

        return builder;
    }
}