using ErrorOr;
using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Vh.Application.Common.Interfaces;


namespace Nexticz.Module.Vh.Application.LoadingActionsNdas.Queries.GetLoadingActionsNdas;

public class GetLoadingActionsNdasQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetLoadingActionsNdasQuery, ErrorOr<FilteredResult>>
{
    public async Task<ErrorOr<FilteredResult>> Handle(GetLoadingActionsNdasQuery query, CancellationToken cancellationToken)
    {
        return await unitOfWork.LoadingActionsNdasRepository.GetLoadingActionsNdasAsync(query.FilteringParams,
            cancellationToken);
    }
}