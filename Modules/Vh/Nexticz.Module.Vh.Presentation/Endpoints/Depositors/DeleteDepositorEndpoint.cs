using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Vh.Application.Depositors.Commands.DeleteDepositor;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;


namespace Nexticz.Module.Vh.Presentation.Endpoints.Depositors;

public static class DeleteDepositorEndpoint
{
    public static IEndpointRouteBuilder MapDeleteDepositor(this IEndpointRouteBuilder builder)
    {
        builder.MapDelete(ApiEndpoints.Depositors.DeleteDepositor,
                async (Guid id, ISender mediatr, CancellationToken cancellationToken) =>
                {
                    var command = new DeleteDepositorCommand { Id = id };
                    var result = await mediatr.Send(command, cancellationToken);

                    return result.Match(
                        _ => Results.NoContent(),
                        ResultsHelper.Problem);
                })
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound)
            .HasApiVersion(1.0)
            .WithName(nameof(ApiEndpoints.Depositors.DeleteDepositor));

        return builder;
    }
}