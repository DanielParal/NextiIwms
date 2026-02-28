using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Vh.Application.SystemActivities.Commands.UpdateSystemActivity;
using Nexticz.Module.Vh.Contracts.SystemActivities;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;


namespace Nexticz.Module.Vh.Presentation.Endpoints.SystemActivities;

public static class UpdateSystemActivityEndpoint
{
    public static IEndpointRouteBuilder MapUpdateSystemActivity(this IEndpointRouteBuilder builder)
    {
        builder.MapPut(ApiEndpoints.SystemActivities.UpdateSystemActivity,
                async (Guid id, UpdateSystemActivityRequest updateSystemActivityRequest, ISender mediatr,
                    CancellationToken cancellationToken) =>
                {
                    var command = new UpdateSystemActivityCommand
                        { Id = id, UpdateSystemActivityRequest = updateSystemActivityRequest };
                    var result = await mediatr.Send(command, cancellationToken);

                    return result.Match(
                        _ => Results.NoContent(),
                        ResultsHelper.Problem);
                })
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound)
            .HasApiVersion(1.0)
            .WithName(nameof(ApiEndpoints.SystemActivities.UpdateSystemActivity));

        return builder;
    }
}