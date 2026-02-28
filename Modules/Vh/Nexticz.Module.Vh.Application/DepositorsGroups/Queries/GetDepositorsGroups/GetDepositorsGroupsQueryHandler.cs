using ErrorOr;
using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Vh.Application.Common.Interfaces;


namespace Nexticz.Module.Vh.Application.DepositorsGroups.Queries.GetDepositorsGroups;

public class GetDepositorsGroupsQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetDepositorsGroupsQuery, ErrorOr<FilteredResult>>
{
    public async Task<ErrorOr<FilteredResult>> Handle(GetDepositorsGroupsQuery query,
        CancellationToken cancellationToken)
    {
        return await unitOfWork.DepositorsGroupsRepository.GetDepositorsGroupsAsync(query.FilteringParams,
            cancellationToken);
    }
}