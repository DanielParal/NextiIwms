using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Vh.Application.WorkerShifts.Commands.AddWorkerShiftActivity;
using Nexticz.Module.Vh.Contracts.WorkerShifts;
using Nexticz.Lib.Shared.Helpers;

namespace Nexticz.Module.Vh.Presentation.Endpoints.WorkerShifts;

public static class AddWorkerShiftActivityEndpoint
{
    public static IEndpointRouteBuilder MapAddWorkershiftActivity(this IEndpointRouteBuilder builder)
    {
        builder.MapPost(ApiEndpoints.WorkerShifts.AddWorkerShiftActivity,
                async (Guid id, AddWorkerShiftActivityRequest addWorkerShiftActivityRequest, ISender mediatr,
                    CancellationToken cancellationToken) =>
                {
                    var command = new AddWorkerShiftActivityCommand(addWorkerShiftActivityRequest, id);

                    var result = await mediatr.Send(command, cancellationToken);

                    return result.Match(
                        _ => Results.Created(),
                        ResultsHelper.Problem);
                })
            .Produces(StatusCodes.Status201Created)
            .HasApiVersion(1.0)
            .WithName(nameof(ApiEndpoints.WorkerShifts.AddWorkerShiftActivity));

        return builder;
    }
}