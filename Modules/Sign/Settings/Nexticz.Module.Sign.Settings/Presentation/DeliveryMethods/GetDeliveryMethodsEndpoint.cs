using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Sign.Settings.Contracts.DeliveryMethods;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Lib.Shared.Extensions;
using Nexticz.Module.Sign.Settings.Application.DeliveryMethods.Queries.GetDeliveryMethods;
using Nexticz.Module.Sign.Settings.Application.Grouping;
using Nexticz.Module.Sign.Settings.Domain.DeliveryMethodAggregate;


namespace Nexticz.Module.Sign.Settings.Presentation.DeliveryMethods;

internal static class GetDeliveryMethodsEndpoint
{
    public static IEndpointRouteBuilder MapGetDeliveryMethodsEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(SettingsEndpoints.DeliveryMethodEndpoints.GetDeliveryMethods,
                async (
                    [AsParameters] BaseFilteringParams filteringParams,
                    ISender sender,
                    CancellationToken cancellationToken,
                    [FromServices] ISettingsGroupedResultHandler groupedResultHandler) =>
                {
                    if (filteringParams.Group is not null)
                    {
                        return Results.Ok(await groupedResultHandler.GetGroupedResultAsync<DeliveryMethod>(filteringParams, cancellationToken));
                    }
                    
                    var result = await sender.Send(new GetDeliveryMethodsQuery(filteringParams), cancellationToken);
                    
                    return Results.Ok(result.MapDataFromTInToTOut(DeliveryMethodResponseFactory.Create));
                })
            .Produces<FilteredResult<DeliveryMethodResponse>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.DeliveryMethodEndpoints.GetDeliveryMethods)));

        return builder;
    }
}