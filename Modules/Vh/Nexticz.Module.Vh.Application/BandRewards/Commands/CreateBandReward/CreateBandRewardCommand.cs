using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Contracts.BandRewards;

namespace Nexticz.Module.Vh.Application.BandRewards.Commands.CreateBandReward;

public class CreateBandRewardCommand : IRequest<ErrorOr<Created>>
{
    public required CreateBandRewardRequest CreateBandRewardRequest { get; set; }
}