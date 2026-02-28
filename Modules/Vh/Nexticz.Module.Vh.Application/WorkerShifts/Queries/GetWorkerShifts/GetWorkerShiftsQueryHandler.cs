using ErrorOr;
using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Vh.Application.Common.Interfaces;


namespace Nexticz.Module.Vh.Application.WorkerShifts.Queries.GetWorkerShifts;

public class GetWorkerShiftsQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetWorkerShiftsQuery, ErrorOr<FilteredResult>>
{
    public async Task<ErrorOr<FilteredResult>> Handle(GetWorkerShiftsQuery query, CancellationToken cancellationToken)
    {
        return await unitOfWork.WorkerShiftsRepository.GetWorkerShiftsAsync(query.FilteringParams, cancellationToken);
    }
}