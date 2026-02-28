using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Vh.Application.NonDispensingActivities.Commands.UpdateNonDispensingActivity;
using Nexticz.Module.Vh.Contracts.NonDispensingActivities;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;


namespace Nexticz.Module.Vh.Presentation.Endpoints.NonDispensingActivities;

public static class UpdateNonDispensingActivityEndpoint
{
    public static IEndpointRouteBuilder MapUpdateNonDispensingActivity(this IEndpointRouteBuilder builder)
    {
        builder.MapPut(ApiEndpoints.NonDispensingActivities.UpdateNonDispensingActivity,
                async (Guid id, UpdateNonDispensingActivityRequest updateNonDispensingActivityRequest, ISender mediatr,
                    CancellationToken cancellationToken) =>
                {
                    var command = new UpdateNonDispensingActivityCommand { Id = id, UpdateNonDispensingActivityRequest = updateNonDispensingActivityRequest };
                    var result = await mediatr.Send(command, cancellationToken);

                    return result.Match(
                        _ => Results.NoContent(),
                        ResultsHelper.Problem);
                })
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound)
            .HasApiVersion(1.0)
            .WithName(nameof(ApiEndpoints.NonDispensingActivities.UpdateNonDispensingActivity));

        return builder;
    }
}