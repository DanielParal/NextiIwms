using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Vh.Application.ActivityCategories.Commands.UpdateActivityCategory;
using Nexticz.Module.Vh.Contracts.ActivityCategories;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;


namespace Nexticz.Module.Vh.Presentation.Endpoints.ActivityCategories;

public static class UpdateActivityCategoryEndpoint
{
    public static IEndpointRouteBuilder MapUpdateActivityCategory(this IEndpointRouteBuilder builder)
    {
        builder.MapPut(ApiEndpoints.ActivityCategories.UpdateActivityCategory,
                async (Guid id, UpdateActivityCategoryRequest updateActivityCategoryRequest, ISender mediatr,
                    CancellationToken cancellationToken) =>
                {
                    var command = new UpdateActivityCategoryCommand
                        { Id = id, UpdateActivityCategoryRequest = updateActivityCategoryRequest };
                    var result = await mediatr.Send(command, cancellationToken);

                    return result.Match(
                        _ => Results.NoContent(),
                        ResultsHelper.Problem);
                })
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound)
            .HasApiVersion(1.0)
            .WithName(nameof(ApiEndpoints.ActivityCategories.UpdateActivityCategory));

        return builder;
    }
}