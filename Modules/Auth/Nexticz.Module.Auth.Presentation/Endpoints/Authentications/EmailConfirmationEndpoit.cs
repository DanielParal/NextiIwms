using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Auth.Application.Authentications.Commands.ConfirmEmail;
using Nexticz.Module.Auth.Contracts.Authentications;
using Nexticz.Lib.Shared.Helpers;

namespace Nexticz.Module.Auth.Presentation.Endpoints.Authentications;

public static class EmailConfirmationEndpoit
{
    public static IEndpointRouteBuilder MapConfirmEmail(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(ApiEndpoints.Authentications.EmailConfirmation, async ([AsParameters]EmailConfirmationRequest request, ISender mediator, CancellationToken cancellationToken) =>
            {
                var command = new EmailConfirmationCommand { Token = request.Token, Email = request.Email };
                var result = await mediator.Send(command, cancellationToken);

                return result.Match(_ => Results.NoContent(), ResultsHelper.Problem);

            }).HasApiVersion(1.0)
            .WithName(nameof(ApiEndpoints.Authentications.EmailConfirmation));
        
        return builder;
    }
}