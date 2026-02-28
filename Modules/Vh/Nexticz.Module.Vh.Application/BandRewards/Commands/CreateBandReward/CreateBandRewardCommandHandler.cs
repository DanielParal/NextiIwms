using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Domain.BandRewards;
using Nexticz.Module.Vh.Application.Common.Interfaces;

namespace Nexticz.Module.Vh.Application.BandRewards.Commands.CreateBandReward;

public class CreateBandRewardCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<CreateBandRewardCommand, ErrorOr<Created>>
{
    public async Task<ErrorOr<Created>> Handle(CreateBandRewardCommand command, CancellationToken cancellationToken)
    {
        var createdBandReward = new BandReward
        {
            Band = command.CreateBandRewardRequest.Band,
            BandNumber = command.CreateBandRewardRequest.BandNumber,
            MinValue = command.CreateBandRewardRequest.MinValue,
            MaxValue = command.CreateBandRewardRequest.MaxValue,
            Reward = command.CreateBandRewardRequest.Reward
        };

        unitOfWork.Add(createdBandReward);

        await unitOfWork.CompleteAsync(cancellationToken);

        return Result.Created;
    }
}