using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Vh.Application.BandRewards.Common.Models;
using Nexticz.Module.Vh.Application.BandRewards.Queries.GetBandRewards;
using Nexticz.Module.Vh.Contracts.BandRewards;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Lib.Shared.Helpers;


namespace Nexticz.Module.Vh.Presentation.Endpoints.BandRewards;

public static class GetBandRewardsEndpoint
{
    public static IEndpointRouteBuilder MapGetBandRewards(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(ApiEndpoints.BandRewards.GetBandRewards,
                async ([AsParameters] BandRewardsFilteringParams filteringParams, ISender mediatr,
                    CancellationToken cancellationToken) =>
                {
                    var query = new GetBandRewardsQuery { FilteringParams = filteringParams };
                    var result = await mediatr.Send(query, cancellationToken);

                    return result.Match(
                        Results.Ok,
                        ResultsHelper.Problem);
                })
            .Produces<FilteredResult<BandRewardResponse>>()
            .HasApiVersion(1.0)
            .WithName(nameof(ApiEndpoints.BandRewards.GetBandRewards));

        return builder;
    }
}