using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Vh.Application.LoadedActivities.Common.Models;
using Nexticz.Module.Vh.Application.LoadedActivities.Queries.GetLoadedActivities;
using Nexticz.Module.Vh.Contracts.LoadedActivities;
using Nexticz.Module.Vh.Domain.LoadedActivities;
using Nexticz.Module.Vh.Presentation.Endpoints.LoadedActivities.Mappers;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Lib.Shared.Helpers;


namespace Nexticz.Module.Vh.Presentation.Endpoints.LoadedActivities;

public static class GetLoadedActivitiesEndpoint
{
    public static IEndpointRouteBuilder MapGetLoadedActivities(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(ApiEndpoints.LoadedActivities.GetLoadedActivities,
                async ([AsParameters] LoadedActivitiesFilteringParams filteringParams, ISender mediatr,
                    CancellationToken cancellationToken) =>
                {
                    var query = new GetLoadedActivitiesQuery { FilteringParams = filteringParams };
                    var result = await mediatr.Send(query, cancellationToken);

                    if (!result.IsError && result.Value.data.GetType() == typeof(LoadedActivity))
                        result.Value.data = result.Value.data.OfType<LoadedActivity>()
                            .Select(x => x.MapToLoadedActivityResponse());

                    return result.Match(
                        Results.Ok,
                        ResultsHelper.Problem);
                })
            .Produces<FilteredResult<LoadedActivityResponse>>()
            .HasApiVersion(1.0)
            .WithName(nameof(ApiEndpoints.LoadedActivities.GetLoadedActivities));

        return builder;
    }
}