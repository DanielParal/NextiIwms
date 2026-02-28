using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Vh.Application.Centers.Commands.UpdateCenter;
using Nexticz.Module.Vh.Contracts.Centers;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;


namespace Nexticz.Module.Vh.Presentation.Endpoints.Centers;

public static class UpdateCenterEndpoint
{
    public static IEndpointRouteBuilder MapUpdateCenter(this IEndpointRouteBuilder builder)
    {
        builder.MapPut(ApiEndpoints.Centers.UpdateCenter, 
            async (Guid id, UpdateCenterRequest updateCenterRequest, ISender mediatr, CancellationToken cancellationToken ) =>
            {
                var command = new UpdateCenterCommand { Id = id, UpdateCenterRequest = updateCenterRequest };
                var result = await mediatr.Send(command, cancellationToken);

                return result.Match(
                    _ => Results.NoContent(),
                    ResultsHelper.Problem);
            })
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound)
            .HasApiVersion(1.0)
            .WithName(nameof(ApiEndpoints.Centers.UpdateCenter));
        
        return builder;
    }
}