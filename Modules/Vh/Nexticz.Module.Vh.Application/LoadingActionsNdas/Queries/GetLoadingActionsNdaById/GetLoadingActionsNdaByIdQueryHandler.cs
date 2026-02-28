using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Contracts.LoadingActionsNdas;
using Nexticz.Module.Vh.Domain.LoadingActionsNdas;
using Nexticz.Module.Vh.Application.Common.Interfaces;

namespace Nexticz.Module.Vh.Application.LoadingActionsNdas.Queries.GetLoadingActionsNdaById;

public class GetLoadingActionsNdaByIdQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetLoadingActionsNdaByIdQuery, ErrorOr<LoadingActionsNdaResponse>>
{
    public async Task<ErrorOr<LoadingActionsNdaResponse>> Handle(GetLoadingActionsNdaByIdQuery query, CancellationToken cancellationToken)
    {
        var loadingActionsNga =
            await unitOfWork.LoadingActionsNdasRepository.GetLoadingActionsNdaResponseByIdAsync(query.Id,
                cancellationToken);

        if (loadingActionsNga is null)
        {
            return LoadingActionsNdaErrors.LoadingActionsNdaWithIdDoesnotExist;
        }

        return loadingActionsNga;
    }
}