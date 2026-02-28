using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Contracts.BandRewards;
using Nexticz.Module.Vh.Domain.BandRewards;
using Nexticz.Module.Vh.Application.Common.Interfaces;

namespace Nexticz.Module.Vh.Application.BandRewards.Queries.GetBandRewardById;

public class GetBandRewardByIdQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetBandRewardByIdQuery, ErrorOr<BandRewardResponse>>
{
    public async Task<ErrorOr<BandRewardResponse>> Handle(GetBandRewardByIdQuery query,
        CancellationToken cancellationToken)
    {
        var bandReward =
            await unitOfWork.BandRewardsRepository.GetBandRewardResponseByIdAsync(query.Id, cancellationToken);

        if (bandReward is null) return BandRewardsErrors.BandRewardWithIdDoesnotExist;

        return bandReward;
    }
}