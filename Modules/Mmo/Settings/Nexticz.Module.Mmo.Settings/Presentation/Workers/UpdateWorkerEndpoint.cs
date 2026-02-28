using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Mmo.Settings.Contracts.Workers;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Mmo.Settings.Application.Workers.Commands.UpdateWorker;


namespace Nexticz.Module.Mmo.Settings.Presentation.Workers;

internal static class UpdateWorkerEndpoint
{
    public static IEndpointRouteBuilder MapUpdateWorker(this IEndpointRouteBuilder builder)
    {
        builder.MapPut(SettingsEndpoints.WorkerEndpoints.UpdateWorker,
                async (
                    Guid id,
                    UpdateWorkerRequest request, 
                    ISender mediator, 
                    CancellationToken cancellationToken) =>
                {
                    var command = new UpdateWorkerCommand(id, request.Name, request.Pin, request.IsActive);
                    
                    var result = await mediator.Send(command, cancellationToken);

                    return result.Match(
                        _ => Results.NoContent(),
                        ResultsHelper.Problem);
                })
            .Produces<object>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound)
            .Produces<ApiErrorResponse>(StatusCodes.Status422UnprocessableEntity)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.WorkerEndpoints.UpdateWorker)));

        return builder;
    }
}