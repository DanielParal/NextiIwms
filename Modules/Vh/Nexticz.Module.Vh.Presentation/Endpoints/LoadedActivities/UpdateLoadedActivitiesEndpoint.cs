using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Vh.Application.LoadedActivities.Commands.UpdateLoadedActivity;
using Nexticz.Module.Vh.Contracts.LoadedActivities;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;


namespace Nexticz.Module.Vh.Presentation.Endpoints.LoadedActivities;

public static class UpdateLoadedActivitiesEndpoint
{
    public static IEndpointRouteBuilder MapUpdateLoadedActivityEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapPut(ApiEndpoints.LoadedActivities.UpdateLoadedActivity,
                async (Guid id, UpdateLoadedActivityRequest updatePartnerRequest, ISender mediatr,
                    CancellationToken cancellationToken) =>
                {
                    var command = new UpdateLoadedActivityCommand(id, updatePartnerRequest);
                    var result = await mediatr.Send(command, cancellationToken);

                    return result.Match(
                        _ => Results.NoContent(),
                        ResultsHelper.Problem);
                })
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound)
            .HasApiVersion(1.0)
            .WithName(nameof(ApiEndpoints.LoadedActivities.UpdateLoadedActivity));

        return builder;
    }
}