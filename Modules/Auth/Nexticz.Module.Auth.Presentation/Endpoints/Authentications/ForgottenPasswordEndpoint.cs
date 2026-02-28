using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Auth.Application.Authentications.Commands.ForgottenPassword;
using Nexticz.Module.Auth.Contracts.Authentications;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;


namespace Nexticz.Module.Auth.Presentation.Endpoints.Authentications;

public static class ForgottenPasswordEndpoint
{
    public static IEndpointRouteBuilder MapForgottenPassword(this IEndpointRouteBuilder builder)
    {
        builder.MapPost(ApiEndpoints.Authentications.ForgottenPassword, async (ForgottenPasswordRequest forgottenPasswordRequest, 
                ISender mediatr, CancellationToken cancellationToken) =>
            {
                var command = new ForgottenPasswordCommand(forgottenPasswordRequest.Email);

                var result = await mediatr.Send(command, cancellationToken);

                return result.Match(
                    _ => Results.NoContent(),
                    ResultsHelper.Problem);
            })
        .HasApiVersion(1.0)
        .Produces(StatusCodes.Status204NoContent)
        .Produces<ApiErrorResponse>(StatusCodes.Status422UnprocessableEntity)
        .Produces(StatusCodes.Status404NotFound)
        .WithName(nameof(ApiEndpoints.Authentications.ForgottenPassword));
        
        return builder;
    }
}