using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Domain.BandRewards;
using Nexticz.Module.Vh.Application.Common.Interfaces;

namespace Nexticz.Module.Vh.Application.BandRewards.Commands.DeleteBandReward;

public class DeleteBandRewardCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteBandRewardCommand, ErrorOr<Deleted>>
{
    public async Task<ErrorOr<Deleted>> Handle(DeleteBandRewardCommand command, CancellationToken cancellationToken)
    {
        var bandReward = await unitOfWork.BandRewardsRepository.GetBandRewardByIdAsync(command.Id, cancellationToken);

        if (bandReward is null) return BandRewardsErrors.BandRewardWithIdDoesnotExist;

        unitOfWork.Remove(bandReward);

        await unitOfWork.CompleteAsync(cancellationToken);

        return Result.Deleted;
    }
}