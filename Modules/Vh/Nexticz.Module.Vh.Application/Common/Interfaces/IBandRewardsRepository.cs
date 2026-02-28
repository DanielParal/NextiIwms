using Nexticz.Module.Vh.Contracts.BandRewards;
using Nexticz.Module.Vh.Domain.BandRewards;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Vh.Application.BandRewards.Common.Models;


namespace Nexticz.Module.Vh.Application.Common.Interfaces;

public interface IBandRewardsRepository
{
    Task<BandReward?> GetBandRewardByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<BandRewardResponse?> GetBandRewardResponseByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<FilteredResult> GetBandRewardsAsync(BandRewardsFilteringParams filteringParams,
        CancellationToken cancellationToken);
}