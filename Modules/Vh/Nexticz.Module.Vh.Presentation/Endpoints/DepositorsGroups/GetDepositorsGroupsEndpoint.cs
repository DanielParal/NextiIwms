using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Vh.Application.DepositorsGroups.Common.Models;
using Nexticz.Module.Vh.Application.DepositorsGroups.Queries.GetDepositorsGroups;
using Nexticz.Module.Vh.Contracts.DepositorsGroups;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Lib.Shared.Helpers;


namespace Nexticz.Module.Vh.Presentation.Endpoints.DepositorsGroups;

public static class GetDepositorsGroupsEndpoint
{
    public static IEndpointRouteBuilder MapGetDepositorsGroups(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(ApiEndpoints.DepositorsGroups.GetDepositorsGroups,
                async ([AsParameters] DepositorsGroupsFilteringParams filteringParams, ISender mediatr,
                    CancellationToken cancellationToken) =>
                {
                    var query = new GetDepositorsGroupsQuery { FilteringParams = filteringParams };
                    var result = await mediatr.Send(query, cancellationToken);

                    return result.Match(
                        Results.Ok,
                        ResultsHelper.Problem);
                })
            .Produces<FilteredResult<DepositorsGroupResponse>>()
            .HasApiVersion(1.0)
            .WithName(nameof(ApiEndpoints.DepositorsGroups.GetDepositorsGroups));
        ;

        return builder;
    }
}