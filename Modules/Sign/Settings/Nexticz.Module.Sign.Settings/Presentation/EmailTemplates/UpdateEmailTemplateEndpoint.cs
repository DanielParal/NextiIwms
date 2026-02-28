using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Sign.Settings.Contracts.EmailTemplates;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Sign.Settings.Application.EmailTemplates.Commands.UpdateEmailTemplate;

namespace Nexticz.Module.Sign.Settings.Presentation.EmailTemplates;

internal static class UpdateEmailTemplateEndpoint
{
    public static IEndpointRouteBuilder MapUpdateEmailTemplateEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapPut(SettingsEndpoints.EmailTemplateEndpoints.UpdateEmailTemplate,
                async (
                    string code, 
                    UpdateEmailTemplateRequest request, 
                    ISender sender,
                    CancellationToken cancellationToken) =>
                {
                    var result = 
                        await sender.Send(new UpdateEmailTemplateCommand(code, request.Name, request.Subject, request.HtmlBody, request.TextBody), 
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
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.EmailTemplateEndpoints.UpdateEmailTemplate)));
        
        return builder;
    }
}