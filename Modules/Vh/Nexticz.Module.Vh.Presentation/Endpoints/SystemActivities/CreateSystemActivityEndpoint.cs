using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Vh.Application.SystemActivities.Commands.CreateSystemActivity;
using Nexticz.Module.Vh.Contracts.SystemActivities;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;


namespace Nexticz.Module.Vh.Presentation.Endpoints.SystemActivities;

public static class CreateSystemActivityEndpoint
{
    public static IEndpointRouteBuilder MapCreateSystemActivity(this IEndpointRouteBuilder builder)
    {
        builder.MapPost(ApiEndpoints.SystemActivities.CreateSystemActivity,
                async (CreateSystemActivityRequest createSystemActivityRequest, ISender mediatr,
                    CancellationToken cancellationToken) =>
                {
                    var command = new CreateSystemActivityCommand
                        { CreateSystemActivityRequest = createSystemActivityRequest };
                    var result = await mediatr.Send(command, cancellationToken);

                    return result.Match(
                        _ => Results.NoContent(),
                        ResultsHelper.Problem);
                })
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ApiErrorResponse>()
            .HasApiVersion(1.0)
            .WithName(nameof(ApiEndpoints.SystemActivities.CreateSystemActivity));

        return builder;
    }
}