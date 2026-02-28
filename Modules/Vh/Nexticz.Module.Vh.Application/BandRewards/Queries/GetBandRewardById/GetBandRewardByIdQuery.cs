using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Contracts.BandRewards;

namespace Nexticz.Module.Vh.Application.BandRewards.Queries.GetBandRewardById;

public class GetBandRewardByIdQuery : IRequest<ErrorOr<BandRewardResponse>>
{
    public required Guid Id { get; set; }
}