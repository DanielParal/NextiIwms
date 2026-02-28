using ErrorOr;
using MediatR;

namespace Nexticz.Module.Vh.Application.BandRewards.Commands.DeleteBandReward;

public class DeleteBandRewardCommand : IRequest<ErrorOr<Deleted>>
{
    public required Guid Id { get; set; }
}