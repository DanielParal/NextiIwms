using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Auth.Application.Accounts.Queries.GetAccountById;
using Nexticz.Module.Auth.Application.Accounts.Queries.GetAccountByUsername;
using Nexticz.Module.Auth.Contracts.Accounts;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;


namespace Nexticz.Module.Auth.Presentation.Endpoints.Accounts;

public static class GetAccountByUsernameOrIdEndpoint
{
    public static IEndpointRouteBuilder MapGetAccountsByUsernameOrId(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(ApiEndpoints.Accounts.GetAccountByUsernameOrId,
                async (string usernameOrId, ISender mediatr, CancellationToken cancellationToken) =>
                {
                    IRequest<ErrorOr<AccountResponse>> query = Guid.TryParse(usernameOrId, out var id)
                        ? new GetAccountByIdQuery(id)
                        : new GetAccountByUsernameQuery(usernameOrId);

                    var result = await mediatr.Send(query, cancellationToken);

                    return result.Match(
                        Results.Ok,
                        ResultsHelper.Problem);
                })
            .Produces<AccountResponse>()
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound)
            .HasApiVersion(1.0)
            .WithName(nameof(ApiEndpoints.Accounts.GetAccountByUsernameOrId));

        return builder;
    }
}