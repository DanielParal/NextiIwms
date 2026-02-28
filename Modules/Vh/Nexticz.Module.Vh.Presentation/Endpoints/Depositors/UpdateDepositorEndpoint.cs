using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Vh.Application.Depositors.Commands.UpdateDepositor;
using Nexticz.Module.Vh.Contracts.Depositors;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;


namespace Nexticz.Module.Vh.Presentation.Endpoints.Depositors;

public static class UpdateDepositorEndpoint
{
    public static IEndpointRouteBuilder MapUpdateDepositor(this IEndpointRouteBuilder builder)
    {
        builder.MapPut(ApiEndpoints.Depositors.UpdateDepositor,
                async (Guid id, UpdateDepositorRequest updateDepositorRequest, ISender mediatr,
                    CancellationToken cancellationToken) =>
                {
                    var command = new UpdateDepositorCommand
                        { Id = id, UpdateDepositorRequest = updateDepositorRequest };
                    var result = await mediatr.Send(command, cancellationToken);

                    return result.Match(
                        _ => Results.NoContent(),
                        ResultsHelper.Problem);
                })
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound)
            .HasApiVersion(1.0)
            .WithName(nameof(ApiEndpoints.Depositors.UpdateDepositor));

        return builder;
    }
}