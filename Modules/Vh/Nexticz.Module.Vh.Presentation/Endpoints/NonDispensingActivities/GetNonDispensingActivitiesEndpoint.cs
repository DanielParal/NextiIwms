using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Vh.Application.NonDispensingActivities.Common.Models;
using Nexticz.Module.Vh.Application.NonDispensingActivities.Queries.GetNonDispensingActivitiesResponse;
using Nexticz.Module.Vh.Contracts.NonDispensingActivities;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Lib.Shared.Helpers;


namespace Nexticz.Module.Vh.Presentation.Endpoints.NonDispensingActivities;

public static class GetNonDispensingActivitiesEndpoint
{
    public static IEndpointRouteBuilder MapGetNonDispensingActivities(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(ApiEndpoints.NonDispensingActivities.GetNonDispensingActivities,
                async ([AsParameters] NonDispensingActivitiesFilteringParams filteringParams, ISender mediatr,
                    CancellationToken cancellationToken) =>
                {
                    var query = new GetNonDispensingActivitiesResponseQuery { FilteringParams = filteringParams };
                    var result = await mediatr.Send(query, cancellationToken);

                    return result.Match(
                        Results.Ok,
                        ResultsHelper.Problem);
                })
            .Produces<FilteredResult<NonDispensingActivityResponse>>()
            .HasApiVersion(1.0)
            .WithName(nameof(ApiEndpoints.NonDispensingActivities.GetNonDispensingActivities));

        return builder;
    }
}