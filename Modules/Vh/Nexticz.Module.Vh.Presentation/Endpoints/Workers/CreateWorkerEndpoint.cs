using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Vh.Application.Workers.Commands.CreateWorker;
using Nexticz.Module.Vh.Contracts.Workers;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;


namespace Nexticz.Module.Vh.Presentation.Endpoints.Workers;

public static class CreateWorkerEndpoint
{
    public static IEndpointRouteBuilder MapCreateWorker(this IEndpointRouteBuilder builder)
    {
        builder.MapPost(ApiEndpoints.Workers.CreateWorker,
                async (CreateWorkerRequest createWorkerRequest, ISender mediatr,
                    CancellationToken cancellationToken) =>
                {
                    var command = new CreateWorkerCommand { CreateWorkerRequest = createWorkerRequest };
                    var result = await mediatr.Send(command, cancellationToken);

                    return result.Match(
                        _ => Results.NoContent(),
                        ResultsHelper.Problem);
                })
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ApiErrorResponse>()
            .HasApiVersion(1.0)
            .WithName(nameof(ApiEndpoints.Workers.CreateWorker));

        return builder;
    }
}