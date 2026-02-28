using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Vh.Application.WorkerShifts.Commands.SetWorkerShiftApproval;
using Nexticz.Module.Vh.Contracts.WorkerShifts;
using Nexticz.Lib.Shared.Helpers;

namespace Nexticz.Module.Vh.Presentation.Endpoints.WorkerShifts;

public static class SetWorkerShiftApprovalEndpoint
{
    public static IEndpointRouteBuilder MapSetWorkerShiftApproval(this IEndpointRouteBuilder builder)
    {
        builder.MapPost(ApiEndpoints.WorkerShifts.SetWorkerShiftApproval,
                async (Guid id, SetWorkerShiftApprovalRequest setWorkerShiftApprovalRequest, ISender mediatr,
                    CancellationToken cancellationToken) =>
                {
                    var command = new SetWorkerShiftApprovalCommand(id, setWorkerShiftApprovalRequest);

                    var result = await mediatr.Send(command, cancellationToken);

                    return result.Match(
                        _ => Results.Created(),
                        ResultsHelper.Problem);
                })
            .Produces(StatusCodes.Status204NoContent)
            .HasApiVersion(1.0)
            .WithName(nameof(ApiEndpoints.WorkerShifts.SetWorkerShiftApproval));

        return builder;
    }
}