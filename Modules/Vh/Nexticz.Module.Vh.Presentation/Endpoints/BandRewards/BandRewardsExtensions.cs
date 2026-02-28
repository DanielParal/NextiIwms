using Microsoft.AspNetCore.Routing;

namespace Nexticz.Module.Vh.Presentation.Endpoints.BandRewards;

public static class BandRewardsExtensions
{
    public static IEndpointRouteBuilder MapGetBandRewardsEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapCreateBandReward()
            .MapUpdateBandReward()
            .MapDeleteBandReward()
            .MapGetBandRewardById()
            .MapGetBandRewards();
    }
}