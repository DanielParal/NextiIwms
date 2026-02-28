using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Auth.Application.Accounts.Commands.CreateAccount;
using Nexticz.Module.Auth.Contracts.Accounts;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;


namespace Nexticz.Module.Auth.Presentation.Endpoints.Accounts;

public static class CraateAccountEndpoint
{
    public static IEndpointRouteBuilder MapCreateAccount(this IEndpointRouteBuilder builder)
    {
        builder.MapPost(ApiEndpoints.Accounts.CreateAccount,
                async (CreateAccountRequest createAccountRequest, IMediator mediatr,
                    CancellationToken cancellationToken) =>
                {
                    var command = new CreateAccountCommand(createAccountRequest);
                    var result = await mediatr.Send(command, cancellationToken);

                    return result.Match(
                        user => Results.Ok(),
                        ResultsHelper.Problem);
                })
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ApiErrorResponse>()
            .HasApiVersion(1.0)
            .WithName(nameof(ApiEndpoints.Accounts.CreateAccount));

        return builder;
    }
}