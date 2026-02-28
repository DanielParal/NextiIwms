using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Mmo.Settings.Contracts.Workers;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Mmo.Settings.Application.Workers;
using Nexticz.Module.Mmo.Settings.Application.Workers.Queries.GetWorkerByPin;

namespace Nexticz.Module.Mmo.Settings.Presentation.Workers;

internal static class GetWorkerByPinEndpoint
{
    public static IEndpointRouteBuilder MapGetWorkerByPin(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(SettingsEndpoints.WorkerEndpoints.GetWorkerByPin,
                async (
                    int pin, 
                    ISender mediator,
                    CancellationToken cancellationToken
                ) =>
                {
                    var request = new GetWorkerByPinQuery(pin);
                    var result = await mediator.Send(request, cancellationToken);
                    return result.Match(
                        worker => Results.Ok(WorkerResponseFactory.Create(worker)),
                        ResultsHelper.Problem);
                })
            .Produces<WorkerResponse>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.WorkerEndpoints.GetWorkerByPin)));

        return builder;
    }
}