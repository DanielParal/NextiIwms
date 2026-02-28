using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Contracts.Workers;
using Nexticz.Module.Vh.Domain.Workers;
using Nexticz.Module.Vh.Application.Common.Interfaces;

namespace Nexticz.Module.Vh.Application.Workers.Queries.GetWorkerById;

public class GetWorkerByIdQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetWorkerByIdQuery, ErrorOr<WorkerResponse>>
{
    public async Task<ErrorOr<WorkerResponse>> Handle(GetWorkerByIdQuery query, CancellationToken cancellationToken)
    {
        var worker = await unitOfWork.WorkersRepository.GetWorkerResponseByIdAsync(query.Id, cancellationToken);

        if (worker is null) return WorkerErrors.WorkerWithIdDoesnotExist;

        return worker;
    }
}