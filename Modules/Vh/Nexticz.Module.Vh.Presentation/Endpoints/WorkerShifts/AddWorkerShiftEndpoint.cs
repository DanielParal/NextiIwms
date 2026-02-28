using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Vh.Application.WorkerShifts.Commands.AddWorkerShift;
using Nexticz.Module.Vh.Contracts.WorkerShifts;
using Nexticz.Lib.Shared.Helpers;

namespace Nexticz.Module.Vh.Presentation.Endpoints.WorkerShifts;

public static class AddWorkerShiftEndpoint
{
    public static IEndpointRouteBuilder MapAddWorkerShift(this IEndpointRouteBuilder builder)
    {
        builder.MapPost(ApiEndpoints.WorkerShifts.AddWorkerShift,
                async (AddWorkerShiftRequest addWorkerShiftRequest, ISender mediatr,
                    CancellationToken cancellationToken) =>
                {
                    var command = new AddWorkerShiftCommand(addWorkerShiftRequest);

                    var result = await mediatr.Send(command, cancellationToken);

                    return result.Match(
                        _ => Results.Created(),
                        ResultsHelper.Problem);
                })
            .Produces(StatusCodes.Status201Created)
            .HasApiVersion(1.0)
            .WithName(nameof(ApiEndpoints.WorkerShifts.AddWorkerShift));

        return builder;
    }
}