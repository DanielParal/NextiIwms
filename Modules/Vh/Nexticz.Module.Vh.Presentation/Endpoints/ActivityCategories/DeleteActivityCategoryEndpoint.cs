using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Vh.Application.ActivityCategories.Commands.DeleteActivityCategory;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;


namespace Nexticz.Module.Vh.Presentation.Endpoints.ActivityCategories;

public static class DeleteActivityCategoryEndpoint
{
    public static IEndpointRouteBuilder MapDeleteActivityCategory(this IEndpointRouteBuilder builder)
    {
        builder.MapDelete(ApiEndpoints.ActivityCategories.DeleteActivityCategory,
                async (Guid id, ISender mediatr, CancellationToken cancellationToken) =>
                {
                    var command = new DeleteActivityCategoryCommand { Id = id };
                    var result = await mediatr.Send(command, cancellationToken);

                    return result.Match(
                        _ => Results.NoContent(),
                        ResultsHelper.Problem);
                })
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound)
            .HasApiVersion(1.0)
            .WithName(nameof(ApiEndpoints.ActivityCategories.DeleteActivityCategory));

        return builder;
    }
}