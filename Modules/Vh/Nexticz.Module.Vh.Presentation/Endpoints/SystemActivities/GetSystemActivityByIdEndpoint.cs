using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Vh.Application.SystemActivities.Queries.GetSystemActivityById;
using Nexticz.Module.Vh.Contracts.SystemActivities;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;


namespace Nexticz.Module.Vh.Presentation.Endpoints.SystemActivities;

public static class GetSystemActivityByIdEndpoint
{
    public static IEndpointRouteBuilder MapGetSystemActivityById(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(ApiEndpoints.SystemActivities.GetSystemActivityById,
                async (Guid id, ISender mediatr, CancellationToken cancellationToken) =>
                {
                    var query = new GetSystemActivityByIdQuery { Id = id };
                    var result = await mediatr.Send(query, cancellationToken);

                    return result.Match(
                        Results.Ok,
                        ResultsHelper.Problem);
                })
            .Produces<SystemActivityResponse>()
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound)
            .HasApiVersion(1.0)
            .WithName(nameof(ApiEndpoints.SystemActivities.GetSystemActivityById));

        return builder;
    }
}