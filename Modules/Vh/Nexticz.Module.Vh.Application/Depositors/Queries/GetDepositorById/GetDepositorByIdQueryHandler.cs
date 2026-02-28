using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Contracts.Depositors;
using Nexticz.Module.Vh.Domain.Depositors;
using Nexticz.Module.Vh.Application.Common.Interfaces;

namespace Nexticz.Module.Vh.Application.Depositors.Queries.GetDepositorById;

public class GetDepositorByIdQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetDepositorByIdQuery, ErrorOr<DepositorResponse>>
{
    public async Task<ErrorOr<DepositorResponse>> Handle(GetDepositorByIdQuery query,
        CancellationToken cancellationToken)
    {
        var depositor =
            await unitOfWork.DepositorsRepository.GetDepositorResponseByIdAsync(query.Id, cancellationToken);

        if (depositor is null) return DepositorErrors.DepositorWithIdDoesnotExist;

        return depositor;
    }
}