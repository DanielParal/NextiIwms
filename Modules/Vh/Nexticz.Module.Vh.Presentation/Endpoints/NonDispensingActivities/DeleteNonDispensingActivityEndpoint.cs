using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Vh.Application.NonDispensingActivities.Commands.DeleteNonDispensingActivity;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;


namespace Nexticz.Module.Vh.Presentation.Endpoints.NonDispensingActivities;

public static class DeleteNonDispensingActivityEndpoint
{
    public static IEndpointRouteBuilder MapDeleteNonDispensingActivity(this IEndpointRouteBuilder builder)
    {
        builder.MapDelete(ApiEndpoints.NonDispensingActivities.DeleteNonDispensingActivity,
                async (Guid id, ISender mediatr, CancellationToken cancellationToken) =>
                {
                    var command = new DeleteNonDispensingActivityCommand { Id = id };
                    var result = await mediatr.Send(command, cancellationToken);

                    return result.Match(
                        _ => Results.NoContent(),
                        ResultsHelper.Problem);
                })
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound)
            .HasApiVersion(1.0)
            .WithName(nameof(ApiEndpoints.NonDispensingActivities.DeleteNonDispensingActivity));

        return builder;
    }
}