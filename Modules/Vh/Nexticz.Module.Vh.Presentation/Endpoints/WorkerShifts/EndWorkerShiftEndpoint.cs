using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Vh.Application.WorkerShifts.Commands.EndWorkerShift;
using Nexticz.Module.Vh.Contracts.WorkerShifts;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;


namespace Nexticz.Module.Vh.Presentation.Endpoints.WorkerShifts;

public static class EndWorkerShiftEndpoint
{
    public static IEndpointRouteBuilder MapEndWorkerShift(this IEndpointRouteBuilder builder)
    {
        builder.MapPost(ApiEndpoints.WorkerShifts.EndWorkerShift, 
                async (Guid id, EndWorkerShiftRequest endWorkerShiftRequest, ISender mediatr, CancellationToken cancellationToken) =>
                {
                    var command = new EndWorkerShiftCommand(id, endWorkerShiftRequest.End);

                    var result = await mediatr.Send(command, cancellationToken);

                    return result.Match(
                        _ => Results.Created(),
                        ResultsHelper.Problem);
                })
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ApiErrorResponse>()
            .HasApiVersion(1.0)
            .WithName(nameof(ApiEndpoints.WorkerShifts.EndWorkerShift));

        return builder;
    }
}