using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Vh.Application.Centers.Commands.CreateCenter;
using Nexticz.Module.Vh.Contracts.Centers;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;


namespace Nexticz.Module.Vh.Presentation.Endpoints.Centers;

public static class CreateCenterEndpoint
{
    public static IEndpointRouteBuilder MapCreateCenter(this IEndpointRouteBuilder builder)
    {
        builder.MapPost(ApiEndpoints.Centers.CreateCenter,
                async (CreateCenterRequest createCenterRequest, ISender mediatr, CancellationToken cancellationToken) =>
                {
                    var command = new CreateCenterCommand { CreateCenterRequest = createCenterRequest };
                    var result = await mediatr.Send(command, cancellationToken);

                    return result.Match(
                        _ => Results.Created(),
                        ResultsHelper.Problem);
                })
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ApiErrorResponse>()
            .HasApiVersion(1.0)
            .WithName(nameof(ApiEndpoints.Centers.CreateCenter));

        return builder;
    }
}