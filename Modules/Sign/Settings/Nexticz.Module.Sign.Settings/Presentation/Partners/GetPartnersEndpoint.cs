using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Sign.Settings.Contracts.Partners;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Lib.Shared.Extensions;
using Nexticz.Module.Sign.Settings.Application.Grouping;
using Nexticz.Module.Sign.Settings.Application.Partners.Queries.GetPartners;
using Nexticz.Module.Sign.Settings.Domain.PartnerAggregate;


namespace Nexticz.Module.Sign.Settings.Presentation.Partners;

internal static class GetPartnersEndpoint
{
    public static IEndpointRouteBuilder MapGetPartnersEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(SettingsEndpoints.PartnerEndpoints.GetPartners,
                async (
                    [AsParameters] BaseFilteringParams filteringParams,
                    ISender sender,
                    CancellationToken cancellationToken,
                    [FromServices] ISettingsGroupedResultHandler groupedResultHandler) =>
                {
                    if (filteringParams.Group is not null)
                    {
                        return Results.Ok(await groupedResultHandler.GetGroupedResultAsync<Partner>(filteringParams, cancellationToken));
                    }
                    
                    var result = await sender.Send(new GetPartnersQuery(filteringParams), cancellationToken);
                    
                    return Results.Ok(result.MapDataFromTInToTOut(PartnerResponseFactory.Create));
                })
            .Produces<FilteredResult<PartnerResponse>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.PartnerEndpoints.GetPartners)));

        return builder;
    }
}