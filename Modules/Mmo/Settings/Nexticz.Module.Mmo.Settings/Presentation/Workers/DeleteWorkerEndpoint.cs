using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Mmo.Settings.Application.Workers.Commands.DeleteWorker;

namespace Nexticz.Module.Mmo.Settings.Presentation.Workers;

internal static class DeleteWorkerEndpoint
{
    public static IEndpointRouteBuilder MapDeleteWorker(this IEndpointRouteBuilder builder)
    {
        builder.MapDelete(SettingsEndpoints.WorkerEndpoints.DeleteWorker,
                async (
                    Guid id,
                    ISender mediator, 
                    CancellationToken cancellationToken) =>
                {
                    var command = new DeleteWorkerCommand(id);
                    var result = await mediator.Send(command, cancellationToken);

                    return result.Match(
                        _ => Results.NoContent(),
                        ResultsHelper.Problem);
                })
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.WorkerEndpoints.DeleteWorker)));

        return builder;
    }
}