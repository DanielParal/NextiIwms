using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Vh.Application.Workers.Queries.GetWorkerById;
using Nexticz.Module.Vh.Application.Workers.Queries.GetWorkerBySlug;
using Nexticz.Module.Vh.Contracts.Workers;

namespace Nexticz.Module.Vh.Presentation.Endpoints.Workers;

public static class GetWorkerByIdOrSlugEndpoint
{
    public static IEndpointRouteBuilder MapGetWorkerByIdOrSlug(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(ApiEndpoints.Workers.GetWorkerByIdOrSlug,
                async (string idOrSlug, ISender mediatr, CancellationToken cancellationToken) =>
                {
                    var getWorkerById = Guid.TryParse(idOrSlug, out var id);

                    var queryById = new GetWorkerByIdQuery { Id = id };
                    var queryBySlug = new GetWorkerBySlugQuery { Slug = idOrSlug };

                    var result = getWorkerById
                        ? await mediatr.Send(queryById, cancellationToken)
                        : await mediatr.Send(queryBySlug, cancellationToken);

                    return result.Match(
                        Results.Ok,
                        _ => Results.NoContent());
                })
            .Produces<WorkerResponse>()
            .Produces(StatusCodes.Status204NoContent)
            .HasApiVersion(1.0)
            .AllowAnonymous()
            .WithName(nameof(ApiEndpoints.Workers.GetWorkerByIdOrSlug));

        return builder;
    }
}