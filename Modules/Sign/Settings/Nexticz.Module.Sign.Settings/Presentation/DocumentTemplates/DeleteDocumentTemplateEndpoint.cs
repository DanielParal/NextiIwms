using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Sign.Settings.Application.DocumentTemplates.Commands.DeleteDocumentTemplate;

namespace Nexticz.Module.Sign.Settings.Presentation.DocumentTemplates;

internal static class DeleteDocumentTemplateEndpoint
{
    public static IEndpointRouteBuilder MapDeleteDocumentTemplateEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapDelete(SettingsEndpoints.DocumentTemplateEndpoints.DeleteDocumentTemplate,
                async (
                    string code, 
                    ISender sender,
                    CancellationToken cancellationToken) =>
                {
                    var result = await sender.Send(new DeleteDocumentTemplateCommand(code), cancellationToken);

                    return result.Match(
                        _ => Results.NoContent(),
                        ResultsHelper.Problem);
                })
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.DocumentTemplateEndpoints.DeleteDocumentTemplate)));

        return builder;
    }
}