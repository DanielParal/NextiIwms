using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Auth.Application.Authentications.Commands.ResetPassword;
using Nexticz.Module.Auth.Contracts.Authentications;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;


namespace Nexticz.Module.Auth.Presentation.Endpoints.Authentications;

public static class ResetPasswordEndpoint
{
    public static IEndpointRouteBuilder MapResetPassword(this IEndpointRouteBuilder builder)
    {
        builder.MapPost(ApiEndpoints.Authentications.ResetPassword, async (ResetPasswordRequest resetPasswordRequest,
                ISender mediatr, CancellationToken cancellationToken) =>
            {
                var command = new ResetPasswordCommand(resetPasswordRequest.Email, resetPasswordRequest.Token,
                    resetPasswordRequest.Password);

                var result = await mediatr.Send(command, cancellationToken);

                return result.Match(
                    _ => Results.NoContent(),
                    ResultsHelper.Problem);
            }).HasApiVersion(1.0)
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ApiErrorResponse>(StatusCodes.Status422UnprocessableEntity)
            .Produces(StatusCodes.Status404NotFound)
            .WithName(nameof(ApiEndpoints.Authentications.ResetPassword));

        return builder;
    }
}