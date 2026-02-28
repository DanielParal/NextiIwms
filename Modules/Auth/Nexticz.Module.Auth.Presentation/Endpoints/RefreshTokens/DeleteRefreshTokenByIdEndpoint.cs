using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Auth.Application.RefreshTokens.Commands.DeleteRefreshTokenById;
using Nexticz.Lib.Shared.Helpers;

namespace Nexticz.Module.Auth.Presentation.Endpoints.RefreshTokens;

public static class DeleteRefreshTokenByIdEndpoint
{
    public static IEndpointRouteBuilder MapDeleteRefreshTokenById(this IEndpointRouteBuilder builder)
    {
        builder.MapDelete(ApiEndpoints.RefreshTokens.DeleteRefreshToken,
            async (Guid id, ISender mediatr, CancellationToken cancellationToken) =>
            {
                var command = new DeleteRefreshTokenByIdCommand(id);
                var result = await mediatr.Send(command, cancellationToken);

                return result.Match(
                    Results.Ok,
                    ResultsHelper.Problem);
            })
            .Produces(StatusCodes.Status204NoContent)
            .HasApiVersion(1.0)
            .WithName(nameof(ApiEndpoints.RefreshTokens.DeleteRefreshToken));

        return builder;
    }
}