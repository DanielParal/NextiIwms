using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Sign.Settings.Contracts.EmailTemplates;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Sign.Settings.Application.EmailTemplates.Queries.GetEmailTemplates;
using Nexticz.Module.Sign.Settings.Application.Grouping;
using Nexticz.Module.Sign.Settings.Domain.EmailTemplateAggregate;

namespace Nexticz.Module.Sign.Settings.Presentation.EmailTemplates;

internal static class GetEmailTemplatesEndpoint
{
    public static IEndpointRouteBuilder MapGetEmailTemplatesEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(SettingsEndpoints.EmailTemplateEndpoints.GetEmailTemplates,
                async (
                    [AsParameters] BaseFilteringParams filteringParams,
                    ISender sender,
                    CancellationToken cancellationToken,
                    [FromServices] ISettingsGroupedResultHandler groupedResultHandler) =>
                {
                    if (filteringParams.Group is not null)
                    {
                        return Results.Ok(await groupedResultHandler.GetGroupedResultAsync<EmailTemplate>(filteringParams, cancellationToken));
                    }
                    
                    var result = await sender.Send(new GetEmailTemplatesQuery(filteringParams), cancellationToken);
                    
                    return Results.Ok(result.MapDataFromTInToTOut(EmailTemplateResponseFactory.Create));
                })
            .Produces<FilteredResult<EmailTemplateResponse>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.EmailTemplateEndpoints.GetEmailTemplates)));

        return builder;
    }
}