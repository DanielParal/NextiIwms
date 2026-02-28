using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Vh.Application.Workers.Commands.UpdateWorker;
using Nexticz.Module.Vh.Contracts.Workers;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;


namespace Nexticz.Module.Vh.Presentation.Endpoints.Workers;

public static class UpdateWorkerEndpoint
{
    public static IEndpointRouteBuilder MapUpdateWorker(this IEndpointRouteBuilder builder)
    {
        builder.MapPut(ApiEndpoints.Workers.UpdateWorker,
                async (Guid id, UpdateWorkerRequest updateWorkerRequest, ISender mediatr,
                    CancellationToken cancellationToken) =>
                {
                    var command = new UpdateWorkerCommand { Id = id, UpdateWorkerRequest = updateWorkerRequest };
                    var result = await mediatr.Send(command, cancellationToken);

                    return result.Match(
                        _ => Results.NoContent(),
                        ResultsHelper.Problem);
                })
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound)
            .HasApiVersion(1.0)
            .WithName(nameof(ApiEndpoints.Workers.UpdateWorker));

        return builder;
    }
}