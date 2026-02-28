using DevExtreme.AspNet.Data;
using Microsoft.EntityFrameworkCore;
using Nexticz.Module.Vh.Application.BandRewards.Common.Models;
using Nexticz.Module.Vh.Application.Common.Interfaces;
using Nexticz.Module.Vh.Contracts.BandRewards;
using Nexticz.Module.Vh.Domain.BandRewards;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Lib.Shared.Extensions;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Vh.Infrastructure.Common.Persistence;


namespace Nexticz.Module.Vh.Infrastructure.BandRewards.Persistence;

public class BandRewardsRepository(DataContext context) : IBandRewardsRepository
{
    public async Task<BandReward?> GetBandRewardByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await context.BandRewards
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<BandRewardResponse?> GetBandRewardResponseByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await context.BandRewards
            .Where(x => x.Id == id)
            .Select(x => new BandRewardResponse
            {
                Id = x.Id,
                Band = x.Band,
                BandNumber = x.BandNumber,
                MinValue = x.MinValue,
                MaxValue = x.MaxValue,
                Reward = x.Reward
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<FilteredResult> GetBandRewardsAsync(BandRewardsFilteringParams filteringParams,
        CancellationToken cancellationToken)
    {
        var query = context.BandRewards
            .Select(x => new BandRewardResponse
            {
                Id = x.Id,
                Band = x.Band,
                BandNumber = x.BandNumber,
                MinValue = x.MinValue,
                MaxValue = x.MaxValue,
                Reward = x.Reward
            });

        var loadOption = FilteringHelper.CreateLoadOptionsFromFilteringParams(filteringParams);
        var loadResult = await DataSourceLoader.LoadAsync(query, loadOption, cancellationToken);

        return loadResult.MapToFilteredResult();
    }
}