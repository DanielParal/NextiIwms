using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Auth.Application.Accounts.Common.Models;
using Nexticz.Module.Auth.Application.Accounts.Queries.GetAccounts;
using Nexticz.Module.Auth.Contracts.Accounts;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Lib.Shared.Helpers;


namespace Nexticz.Module.Auth.Presentation.Endpoints.Accounts;

public static class GetAccountsEndpoint
{
    public static IEndpointRouteBuilder MapGetAllAccounts(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(ApiEndpoints.Accounts.GetAccounts, async ([AsParameters] AccountsFilteringParams filteringParams,
                ISender mediatr, CancellationToken cancellationToken) =>
            {
                var query = new GetAccountsQuery(filteringParams);
                var result = await mediatr.Send(query, cancellationToken);

                return result.Match(
                    Results.Ok,
                    ResultsHelper.Problem);
            })
            .Produces<FilteredResult<AccountResponse>>()
            .HasApiVersion(1.0)
            .WithName(nameof(ApiEndpoints.Accounts.GetAccounts));

        return builder;
    }
}