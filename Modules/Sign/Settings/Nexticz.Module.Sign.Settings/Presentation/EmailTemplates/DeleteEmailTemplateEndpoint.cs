using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Sign.Settings.Application.EmailTemplates.Commands.DeleteEmailTemplate;

namespace Nexticz.Module.Sign.Settings.Presentation.EmailTemplates;

internal static class DeleteEmailTemplateEndpoint
{
    public static IEndpointRouteBuilder MapDeleteEmailTemplateEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapDelete(SettingsEndpoints.EmailTemplateEndpoints.DeleteEmailTemplate,
                async (
                    string code, 
                    ISender sender,
                    CancellationToken cancellationToken) =>
                {
                    var result = await sender.Send(new DeleteEmailTemplateCommand(code), cancellationToken);

                    return result.Match(
                        _ => Results.NoContent(),
                        ResultsHelper.Problem);
                })
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.EmailTemplateEndpoints.DeleteEmailTemplate)));

        return builder;
    }
}