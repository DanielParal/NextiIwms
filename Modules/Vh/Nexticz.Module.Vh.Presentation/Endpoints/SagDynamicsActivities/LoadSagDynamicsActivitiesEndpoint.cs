using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Vh.Application.SagDynamicsActivities.Command.LoadSagDynamicsActivities;
using Nexticz.Lib.Shared.Helpers;

namespace Nexticz.Module.Vh.Presentation.Endpoints.SagDynamicsActivities;

public static class LoadSagDynamicsActivitiesEndpoint
{
    public static IEndpointRouteBuilder MapLoadSagDynamicsActivities(this IEndpointRouteBuilder builder)
    {
        builder.MapPost(ApiEndpoints.SagDynamicsActivities.LoadDynamicsActivities,
                async (HttpRequest request, ISender mediatr) =>
                {
                    if (!request.HasFormContentType) return Results.BadRequest();

                    var form = await request.ReadFormAsync();

                    var command = new LoadSagDynamicsActivitiesCommand(form);
                    var result = await mediatr.Send(command);

                    return result.Match(
                        Results.Ok,
                        ResultsHelper.Problem);
                })
            .Produces(StatusCodes.Status204NoContent)
            .HasApiVersion(1.0)
            .WithName(nameof(ApiEndpoints.SagDynamicsActivities.LoadDynamicsActivities))
            .Accepts<IFormFileCollection>("multipart/form-data");

        return builder;
    }
}