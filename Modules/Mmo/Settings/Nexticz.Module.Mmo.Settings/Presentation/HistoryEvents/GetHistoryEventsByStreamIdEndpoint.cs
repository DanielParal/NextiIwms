using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Mmo.Settings.Contracts.HistoryEvents;
using Nexticz.Module.Mmo.Settings.Domain;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Lib.Shared.Extensions;
using Nexticz.Module.Mmo.Settings.Application.HistoryEvents.Queries.GetHistoryEventsByStreamId;


namespace Nexticz.Module.Mmo.Settings.Presentation.HistoryEvents;

internal static class GetHistoryEventsByStreamIdEndpoint
{
    public static IEndpointRouteBuilder MapGetHistoryEventsByStreamId(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(SettingsEndpoints.HistoryEventEndpoints.GetHistoryEventsByStreamId,
                async (
                    Guid streamId, 
                    [AsParameters] HistoryEventsFilteringParams filteringParams,
                    ISender mediator,
                    CancellationToken cancellationToken
                ) =>
                {
                    filteringParams.Group = null; // ignore grouping for history events
                    
                    var query = new GetHistoryEventsByStreamIdQuery(streamId, filteringParams);
                    var result = await mediator.Send(query, cancellationToken);
                    
                    return Results.Ok(result.MapDataFromTInToTOut(HistoryEventResponseFactory.Create));
                })
            .Produces<FilteredResult<HistoryEventResponse>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.HistoryEventEndpoints.GetHistoryEventsByStreamId)));

        return builder;
    }
}