using ErrorOr;
using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Vh.Application.Common.Interfaces;


namespace Nexticz.Module.Vh.Application.Workers.Queries.GetWorkers;

public class GetWorkersQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetWorkersQuery, ErrorOr<FilteredResult>>
{
    public async Task<ErrorOr<FilteredResult>> Handle(GetWorkersQuery query, CancellationToken cancellationToken)
    {
        return await unitOfWork.WorkersRepository.GetWorkersAsync(query.FilteringParams, cancellationToken);
    }
}