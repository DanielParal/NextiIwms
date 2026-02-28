using ErrorOr;
using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Vh.Application.Common.Interfaces;


namespace Nexticz.Module.Vh.Application.BandRewards.Queries.GetBandRewards;

public class GetBandRewardsQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetBandRewardsQuery, ErrorOr<FilteredResult>>
{
    public async Task<ErrorOr<FilteredResult>> Handle(GetBandRewardsQuery query, CancellationToken cancellationToken)
    {
        return await unitOfWork.BandRewardsRepository.GetBandRewardsAsync(query.FilteringParams, cancellationToken);
    }
}