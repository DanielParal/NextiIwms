using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Sign.Settings.Contracts.DocumentTemplates;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Sign.Settings.Application.DocumentTemplates;
using Nexticz.Module.Sign.Settings.Application.DocumentTemplates.Commands.CreateDocumentTemplate;

namespace Nexticz.Module.Sign.Settings.Presentation.DocumentTemplates;

internal static class CreateDocumentTemplateEndpoint
{
    public static IEndpointRouteBuilder MapCreateDocumentTemplateEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapPost(SettingsEndpoints.DocumentTemplateEndpoints.CreateDocumentTemplate,
                async (
                    CreateDocumentTemplateRequest request,
                    ISender sender,
                    CancellationToken cancellationToken) =>
                {
                    var result = 
                        await sender.Send(new CreateDocumentTemplateCommand(request.Code, request.TextOffsets, request.TextBackgrounds), 
                            cancellationToken);

                    return result.Match(
                        documentTemplate => Results.Created(
                            $"{SettingsEndpoints.DocumentTemplateEndpoints.GetDocumentTemplates}/{documentTemplate.Id}",
                            DocumentTemplateResponseFactory.Create(documentTemplate)),
                        ResultsHelper.Problem);
                })
            .Produces<DocumentTemplateResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces<ApiErrorResponse>(StatusCodes.Status422UnprocessableEntity)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.DocumentTemplateEndpoints.CreateDocumentTemplate)));

        return builder;
    }
}