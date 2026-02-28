using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Sign.Settings.Contracts.DocumentTemplates;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Sign.Settings.Application.DocumentTemplates.Commands.UpdateDocumentTemplate;

namespace Nexticz.Module.Sign.Settings.Presentation.DocumentTemplates;

internal static class UpdateDocumentTemplateEndpoint
{
    public static IEndpointRouteBuilder MapUpdateDocumentTemplateEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapPut(SettingsEndpoints.DocumentTemplateEndpoints.UpdateDocumentTemplate,
                async (
                    string code, 
                    UpdateDocumentTemplateRequest request, 
                    ISender sender,
                    CancellationToken cancellationToken) =>
                {
                    var result = 
                        await sender.Send(new UpdateDocumentTemplateCommand(code, request.TextOffsets, request.TextBackgrounds), 
                            cancellationToken);

                    return result.Match(
                        _ => Results.NoContent(),
                        ResultsHelper.Problem);
                })
            .Produces<object>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces<ApiErrorResponse>(StatusCodes.Status422UnprocessableEntity)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.DocumentTemplateEndpoints.UpdateDocumentTemplate)));
        
        return builder;
    }
}