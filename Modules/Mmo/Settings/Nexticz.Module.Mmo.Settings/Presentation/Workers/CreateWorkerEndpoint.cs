using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Mmo.Settings.Contracts.Workers;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Mmo.Settings.Application.Workers;
using Nexticz.Module.Mmo.Settings.Application.Workers.Commands.CreateWorker;


namespace Nexticz.Module.Mmo.Settings.Presentation.Workers;

internal static class CreateWorkerEndpoint
{
    public static IEndpointRouteBuilder MapCreateWorker(this IEndpointRouteBuilder builder)
    {
        builder.MapPost(SettingsEndpoints.WorkerEndpoints.CreateWorker,
                async (
                    CreateWorkerRequest request,
                    ISender mediator,
                    CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(
                        new CreateWorkerCommand(request.Name, request.Pin, request.IsActive), 
                        cancellationToken);
        
                    return result.Match(
                        worker => 
                            Results.Created($"/{SettingsEndpoints.WorkerEndpoints.CreateWorker}/{worker.Id}", 
                                WorkerResponseFactory.Create(worker)),
                        ResultsHelper.Problem);
                })
            .Produces<WorkerResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces<ApiErrorResponse>(StatusCodes.Status422UnprocessableEntity)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.WorkerEndpoints.CreateWorker)));

        return builder;
    }
}