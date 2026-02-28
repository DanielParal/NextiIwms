using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Vh.Application.Depositors.Commands.CreateDepositor;
using Nexticz.Module.Vh.Contracts.Depositors;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;


namespace Nexticz.Module.Vh.Presentation.Endpoints.Depositors;

public static class CreateDepositorEndpoint
{
    public static IEndpointRouteBuilder MapCreateDepositor(this IEndpointRouteBuilder builder)
    {
        builder.MapPost(ApiEndpoints.Depositors.CreateDepositor,
                async (CreateDepositorRequest createDepositorRequest, ISender mediatr,
                    CancellationToken cancellationToken) =>
                {
                    var command = new CreateDepositorCommand { CreateDepositorRequest = createDepositorRequest };
                    var result = await mediatr.Send(command, cancellationToken);

                    return result.Match(
                        _ => Results.NoContent(),
                        ResultsHelper.Problem);
                })
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ApiErrorResponse>()
            .HasApiVersion(1.0)
            .WithName(nameof(ApiEndpoints.Depositors.CreateDepositor));

        return builder;
    }
}