using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Auth.Application.Accounts.Commands.UpdateAccount;
using Nexticz.Module.Auth.Application.Accounts.Queries.GetAccountById;
using Nexticz.Module.Auth.Contracts.Accounts;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;

namespace Nexticz.Module.Auth.Presentation.Endpoints.Accounts;

public static class UpdateAccountEndpoint
{
    public static IEndpointRouteBuilder MapUpdateAccount(this IEndpointRouteBuilder builder)
    {
        builder.MapPut(ApiEndpoints.Accounts.UpdateAccount, async (Guid id,
                UpdateAccountRequest updateAccountRequest, IMediator mediatr, CancellationToken cancellationToken) =>
            {
                var command = new UpdateAccountCommand(id, updateAccountRequest);
                var result = await mediatr.Send(command, cancellationToken);

                return result.Match(
                    _ => Results.Ok(),
                    ResultsHelper.Problem);
            })
            .HasApiVersion(1.0)
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ApiErrorResponse>(StatusCodes.Status422UnprocessableEntity)
            .Produces(StatusCodes.Status404NotFound)
            .WithName(nameof(ApiEndpoints.Accounts.UpdateAccount));

        return builder;
    }
}