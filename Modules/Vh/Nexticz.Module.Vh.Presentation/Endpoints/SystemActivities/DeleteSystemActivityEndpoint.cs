using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Vh.Application.SystemActivities.Commands.DeleteSystemActivity;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;


namespace Nexticz.Module.Vh.Presentation.Endpoints.SystemActivities;

public static class DeleteSystemActivityEndpoint
{
    public static IEndpointRouteBuilder MapDeleteSystemActivity(this IEndpointRouteBuilder builder)
    {
        builder.MapDelete(ApiEndpoints.SystemActivities.DeleteSystemActivity,
                async (Guid id, ISender mediatr, CancellationToken cancellationToken) =>
                {
                    var command = new DeleteSystemActivityCommand { Id = id };
                    var result = await mediatr.Send(command, cancellationToken);

                    return result.Match(
                        _ => Results.NoContent(),
                        ResultsHelper.Problem);
                })
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound)
            .HasApiVersion(1.0)
            .WithName(nameof(ApiEndpoints.SystemActivities.DeleteSystemActivity));

        return builder;
    }
}