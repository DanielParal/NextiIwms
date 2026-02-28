using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Vh.Application.SystemActivities.Common.Models;
using Nexticz.Module.Vh.Application.SystemActivities.Queries.GetSystemActivities;
using Nexticz.Module.Vh.Contracts.SystemActivities;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Lib.Shared.Helpers;


namespace Nexticz.Module.Vh.Presentation.Endpoints.SystemActivities;

public static class GetSystemActivitiesEndpoint
{
    public static IEndpointRouteBuilder MapGetSystemActivities(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(ApiEndpoints.SystemActivities.GetSystemActivities,
                async ([AsParameters] SystemActivitiesFilteringParams filteringParams, ISender mediatr,
                    CancellationToken cancellationToken) =>
                {
                    var query = new GetSystemActivitiesQuery { FilteringParams = filteringParams };
                    var result = await mediatr.Send(query, cancellationToken);

                    return result.Match(
                        Results.Ok,
                        ResultsHelper.Problem);
                })
            .Produces<FilteredResult<SystemActivityResponse>>()
            .HasApiVersion(1.0)
            .WithName(nameof(ApiEndpoints.SystemActivities.GetSystemActivities));

        return builder;
    }
}