using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Sign.Settings.Contracts.EmailTemplates;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Sign.Settings.Application.EmailTemplates.Queries.GetEmailTemplateByCode;

namespace Nexticz.Module.Sign.Settings.Presentation.EmailTemplates;

internal static class GetEmailTemplateByIdEndpoint
{
    public static IEndpointRouteBuilder MapGetEmailTemplateByIdEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(SettingsEndpoints.EmailTemplateEndpoints.GetEmailTemplateById,
                async (
                    string code, 
                    ISender sender,
                    CancellationToken cancellationToken
                ) =>
                {
                    var result = 
                        await sender.Send(new GetEmailTemplateByCodeQuery(code), cancellationToken);
                    
                    return result.Match(
                        emailTemplate => Results.Ok(EmailTemplateResponseFactory.Create(emailTemplate)),
                        ResultsHelper.Problem);
                })
            .Produces<EmailTemplateResponse>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.EmailTemplateEndpoints.GetEmailTemplateById)));

        return builder;
    }
}