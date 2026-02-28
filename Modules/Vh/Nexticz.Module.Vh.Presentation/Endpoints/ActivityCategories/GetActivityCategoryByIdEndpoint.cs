using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Vh.Application.ActivityCategories.Queries.GetActivityCategories;
using Nexticz.Module.Vh.Contracts.ActivityCategories;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;


namespace Nexticz.Module.Vh.Presentation.Endpoints.ActivityCategories;

public static class GetActivityCategoryByIdEndpoint
{
    public static IEndpointRouteBuilder MapGetActivityCategoryById(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(ApiEndpoints.ActivityCategories.GetActivityCategoryById, async
                (Guid id, ISender mediatr, CancellationToken cancellationToken) =>
            {
                var query = new GetActivityCategoryByIdQuery { Id = id };
                var result = await mediatr.Send(query, cancellationToken);

                return result.Match(
                    Results.Ok,
                    ResultsHelper.Problem);
            })
            .Produces<ActivityCategoryResponse>()
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound)
            .HasApiVersion(1.0)
            .WithName(nameof(ApiEndpoints.ActivityCategories.GetActivityCategoryById));

        return builder;
    }
}