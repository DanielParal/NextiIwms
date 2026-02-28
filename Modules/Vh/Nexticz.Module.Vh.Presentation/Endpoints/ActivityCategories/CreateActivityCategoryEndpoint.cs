using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Vh.Application.ActivityCategories.Commands.CreateActivityCategory;
using Nexticz.Module.Vh.Contracts.ActivityCategories;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;


namespace Nexticz.Module.Vh.Presentation.Endpoints.ActivityCategories;

public static class CreateActivityCategoryEndpoint
{
    public static IEndpointRouteBuilder MapCreateActivityCategory(this IEndpointRouteBuilder builder)
    {
        builder.MapPost(ApiEndpoints.ActivityCategories.CreateActivityCategory,
                async (CreateActivityCategoryRequest createActivityCategoryRequest, ISender mediatr,
                    CancellationToken cancellationToken) =>
                {
                    var command = new CreateActivityCategoryCommand
                        { CreateActivityCategoryRequest = createActivityCategoryRequest };
                    var result = await mediatr.Send(command, cancellationToken);

                    return result.Match(
                        _ => Results.Created(),
                        ResultsHelper.Problem);
                })
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ApiErrorResponse>()
            .HasApiVersion(1.0)
            .WithName(nameof(ApiEndpoints.ActivityCategories.CreateActivityCategory));

        return builder;
    }
}