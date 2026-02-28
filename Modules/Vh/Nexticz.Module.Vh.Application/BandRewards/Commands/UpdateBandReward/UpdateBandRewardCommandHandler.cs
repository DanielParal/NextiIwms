using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Domain.BandRewards;
using Nexticz.Module.Vh.Application.Common.Interfaces;

namespace Nexticz.Module.Vh.Application.BandRewards.Commands.UpdateBandReward;

public class UpdateBandRewardCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateBandRewardCommand, ErrorOr<Updated>>
{
    public async Task<ErrorOr<Updated>> Handle(UpdateBandRewardCommand command, CancellationToken cancellationToken)
    {
        var bandReward = await unitOfWork.BandRewardsRepository.GetBandRewardByIdAsync(command.Id, cancellationToken);

        if (bandReward is null) return BandRewardsErrors.BandRewardWithIdDoesnotExist;

        bandReward.Band = command.UpdateBandRewardRequest.Band;
        bandReward.BandNumber = command.UpdateBandRewardRequest.BandNumber;
        bandReward.MinValue = command.UpdateBandRewardRequest.MinValue;
        bandReward.MaxValue = command.UpdateBandRewardRequest.MaxValue;
        bandReward.Reward = command.UpdateBandRewardRequest.Reward;

        await unitOfWork.CompleteAsync(cancellationToken);

        return Result.Updated;
    }
}