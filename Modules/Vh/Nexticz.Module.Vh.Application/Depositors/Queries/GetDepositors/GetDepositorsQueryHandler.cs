using ErrorOr;
using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Vh.Application.Common.Interfaces;


namespace Nexticz.Module.Vh.Application.Depositors.Queries.GetDepositors;

public class GetDepositorsQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetDepositorsQuery, ErrorOr<FilteredResult>>
{
    public async Task<ErrorOr<FilteredResult>> Handle(GetDepositorsQuery query, CancellationToken cancellationToken)
    {
        return await unitOfWork.DepositorsRepository.GetDepositorsAsync(query.FilteringParams, cancellationToken);
    }
}