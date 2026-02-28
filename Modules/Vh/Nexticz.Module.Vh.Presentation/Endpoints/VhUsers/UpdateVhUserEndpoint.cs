using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Vh.Application.VhUsers.Commands;
using Nexticz.Module.Vh.Contracts.VhUsers;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;


namespace Nexticz.Module.Vh.Presentation.Endpoints.VhUsers;

public static class UpdateVhUserEndpoint
{
    public static IEndpointRouteBuilder MapUpdateVhUserEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapPut(ApiEndpoints.VhUsers.UpdateVhUser,
                async (UpdateVhUserRequest updateVhUserRequest, ISender mediatr) =>
                {
                    var command = new UpdateVhUserCommand(updateVhUserRequest);
                    var result = await mediatr.Send(command);

                    result.Match(
                        _ => Results.NoContent(),
                        ResultsHelper.Problem);
                })
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound)
            .HasApiVersion(1.0)
            .WithName(nameof(ApiEndpoints.VhUsers.UpdateVhUser));

        return builder;
    }
}