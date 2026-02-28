using ErrorOr;
using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Vh.Application.BandRewards.Common.Models;


namespace Nexticz.Module.Vh.Application.BandRewards.Queries.GetBandRewards;

public class GetBandRewardsQuery : IRequest<ErrorOr<FilteredResult>>
{
    public required BandRewardsFilteringParams FilteringParams { get; set; }
}