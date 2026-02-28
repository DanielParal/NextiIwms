using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Sign.Settings.Contracts.EmailTemplates;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Sign.Settings.Application.EmailTemplates.Commands.CreateEmailTemplate;

namespace Nexticz.Module.Sign.Settings.Presentation.EmailTemplates;

internal static class CreateEmailTemplateEndpoint
{
    public static IEndpointRouteBuilder MapCreateEmailTemplateEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapPost(SettingsEndpoints.EmailTemplateEndpoints.CreateEmailTemplate,
                async (
                    CreateEmailTemplateRequest request,
                    ISender sender,
                    CancellationToken cancellationToken) =>
                {
                    var result = 
                        await sender.Send(new CreateEmailTemplateCommand(request.Code, request.Name, request.Subject, request.HtmlBody, request.TextBody), 
                            cancellationToken);

                    return result.Match(
                        emailTemplate => Results.Created(
                            $"{SettingsEndpoints.EmailTemplateEndpoints.GetEmailTemplates}/{emailTemplate.Id}",
                            EmailTemplateResponseFactory.Create(emailTemplate)),
                        ResultsHelper.Problem);
                })
            .Produces<EmailTemplateResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces<ApiErrorResponse>(StatusCodes.Status422UnprocessableEntity)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.EmailTemplateEndpoints.CreateEmailTemplate)));

        return builder;
    }
}