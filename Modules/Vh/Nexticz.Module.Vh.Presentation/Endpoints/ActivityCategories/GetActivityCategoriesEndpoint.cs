using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Vh.Application.ActivityCategories.Common.Models;
using Nexticz.Module.Vh.Application.ActivityCategories.Queries.GetActivityCategoryById;
using Nexticz.Module.Vh.Contracts.ActivityCategories;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Lib.Shared.Helpers;


namespace Nexticz.Module.Vh.Presentation.Endpoints.ActivityCategories;

public static class GetActivityCategoriesEndpoint
{
    public static IEndpointRouteBuilder MapGetActivityCategories(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(ApiEndpoints.ActivityCategories.GetActivityCategories,
                async ([AsParameters] ActivityCategoriesFilteringParams filteringParams,
                    ISender mediatr, CancellationToken cancellationToken) =>
                {
                    var query = new GetActivityCategoriesQuery { FilteringParams = filteringParams };
                    var result = await mediatr.Send(query, cancellationToken);

                    return result.Match(
                        Results.Ok,
                        ResultsHelper.Problem);
                })
            .Produces<FilteredResult<ActivityCategoryResponse>>()
            .HasApiVersion(1.0)
            .WithName(nameof(ApiEndpoints.ActivityCategories.GetActivityCategories));

        return builder;
    }
}