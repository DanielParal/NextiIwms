using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Vh.Application.NonDispensingActivities.Commands.CreateNonDispensingActivity;
using Nexticz.Module.Vh.Contracts.NonDispensingActivities;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;


namespace Nexticz.Module.Vh.Presentation.Endpoints.NonDispensingActivities;

public static class CreateNonDispensingActivityEndpoint
{
    public static IEndpointRouteBuilder MapCreateNonDispensingActivity(this IEndpointRouteBuilder builder)
    {
        builder.MapPost(ApiEndpoints.NonDispensingActivities.CreateNonDispensingActivity,
                async (CreateNonDispensingActivityRequest createNonDispensingActivityRequest, ISender mediatr,
                    CancellationToken cancellationToken) =>
                {
                    var command = new CreateNonDispensingActivityCommand
                        { CreateNonDispensingActivityRequest = createNonDispensingActivityRequest };
                    var result = await mediatr.Send(command, cancellationToken);

                    return result.Match(
                        _ => Results.NoContent(),
                        ResultsHelper.Problem);
                })
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ApiErrorResponse>()
            .HasApiVersion(1.0)
            .WithName(nameof(ApiEndpoints.NonDispensingActivities.CreateNonDispensingActivity));

        return builder;
    }
}