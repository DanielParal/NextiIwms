using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Sign.Settings.Contracts.EmailConfigurations;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Sign.Settings.Application.EmailConfigurations.Queries.GetEmailConfigurations;
using Nexticz.Module.Sign.Settings.Application.Grouping;
using Nexticz.Module.Sign.Settings.Domain.EmailConfigurationAggregate;

namespace Nexticz.Module.Sign.Settings.Presentation.EmailConfigurations;

internal static class GetEmailConfigurationsEndpoint
{
    public static IEndpointRouteBuilder MapGetEmailConfigurationsEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(SettingsEndpoints.EmailConfigurationEndpoints.GetEmailConfigurations,
                async (
                    [AsParameters] BaseFilteringParams filteringParams,
                    ISender sender,
                    CancellationToken cancellationToken,
                    [FromServices] ISettingsGroupedResultHandler groupedResultHandler) =>
                {
                    if (filteringParams.Group is not null)
                    {
                        return Results.Ok(await groupedResultHandler.GetGroupedResultAsync<EmailConfiguration>(filteringParams, cancellationToken));
                    }
                    
                    var result = await sender.Send(new GetEmailConfigurationsQuery(filteringParams), cancellationToken);
                    
                    return Results.Ok(result.MapDataFromTInToTOut(EmailConfigurationResponseFactory.Create));
                })
            .Produces<FilteredResult<EmailConfigurationResponse>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.EmailConfigurationEndpoints.GetEmailConfigurations)));

        return builder;
    }
}