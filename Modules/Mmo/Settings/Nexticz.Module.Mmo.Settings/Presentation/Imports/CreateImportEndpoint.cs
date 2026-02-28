using DocumentFormat.OpenXml.Math;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Mmo.Settings.Contracts.Imports;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Lib.Shared.UserProviders;
using Nexticz.Module.Mmo.Settings.Application.ImportsExports.Imports;
using Nexticz.Module.Mmo.Settings.Application.ImportsExports.Imports.Commands.CreateImport;
using Nexticz.Module.Mmo.Settings.Domain.ImportEntity;


namespace Nexticz.Module.Mmo.Settings.Presentation.Imports;

internal static class CreateImportEndpoint
{
    public static IEndpointRouteBuilder MapCreateImport(this IEndpointRouteBuilder builder)
    {
        builder.MapPost(SettingsEndpoints.ImportEndpoints.CreateImport,
                async (
                    HttpRequest request, 
                    ISender mediatr,
                    CancellationToken cancellationToken,
                    [FromServices] ICurrentUserProvider currentUserProvider) =>
                {
                    if (!request.HasFormContentType)
                    {
                        return ResultsHelper.Problem(
                            ImportErrors.ValidationMultipartFormRequired);
                    }
        
                    var form = await request.ReadFormAsync(cancellationToken);
        
                    var file = form.Files.GetFile("file");

                    if (file is null || file.Length == 0)
                    {
                        return ResultsHelper.Problem(
                            ImportErrors.ValidationNoFileAttached);
                    }
                    
                    if (!Enum.TryParse<ImportType>(form[nameof(ImportType)], out var importSource))
                    {
                        return ResultsHelper.Problem(
                            ImportErrors.ValidationMissingImportSourceType);
                    }
                    
                    var createCommand = new CreateImportCommand(currentUserProvider.GetCurrentUser().UserName, importSource, file);
                    var result = await mediatr.Send(createCommand, cancellationToken);
                    
                    return result.Match(
                        import => Results.Ok(ImportResponseFactory.Create(import)),
                        ResultsHelper.Problem);
                })
            .Produces<ImportResponse>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces<ApiErrorResponse>(StatusCodes.Status422UnprocessableEntity)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.ImportEndpoints.CreateImport)))
            .Accepts<IFormCollection>("multipart/form-data");

        return builder;
    }
}