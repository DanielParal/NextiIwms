using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Sign.Settings.Contracts.DocumentTemplates;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Sign.Settings.Application.DocumentTemplates;
using Nexticz.Module.Sign.Settings.Application.DocumentTemplates.Queries.GetDocumentTemplateByCode;

namespace Nexticz.Module.Sign.Settings.Presentation.DocumentTemplates;

internal static class GetDocumentTemplateByCodeEndpoint
{
    public static IEndpointRouteBuilder MapGetDocumentTemplateByCodeEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(SettingsEndpoints.DocumentTemplateEndpoints.GetDocumentTemplateByCode,
                async (
                    string code, 
                    ISender sender,
                    CancellationToken cancellationToken
                ) =>
                {
                    var result = 
                        await sender.Send(new GetDocumentTemplateByCodeQuery(code), cancellationToken);
                    
                    return result.Match(
                        documentTemplate => Results.Ok(DocumentTemplateResponseFactory.Create(documentTemplate)),
                        ResultsHelper.Problem);
                })
            .Produces<DocumentTemplateResponse>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.DocumentTemplateEndpoints.GetDocumentTemplateByCode)));

        return builder;
    }
}