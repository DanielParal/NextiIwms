using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Contracts.BandRewards;

namespace Nexticz.Module.Vh.Application.BandRewards.Commands.UpdateBandReward;

public class UpdateBandRewardCommand : IRequest<ErrorOr<Updated>>
{
    public required Guid Id { get; set; }
    public required UpdateBandRewardRequest UpdateBandRewardRequest { get; set; }
}