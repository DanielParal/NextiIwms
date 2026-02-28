using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Contracts.Workers;
using Nexticz.Module.Vh.Domain.Workers;
using Nexticz.Module.Vh.Application.Common.Interfaces;

namespace Nexticz.Module.Vh.Application.Workers.Queries.GetWorkerBySlug;

public class GetWorkerBySlugQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetWorkerBySlugQuery, ErrorOr<WorkerResponse>>
{
    public async Task<ErrorOr<WorkerResponse>> Handle(GetWorkerBySlugQuery query, CancellationToken cancellationToken)
    {
        var worker = await unitOfWork.WorkersRepository.GetWorkerResponseBySlugAsync(query.Slug, cancellationToken);

        if (worker is null) return WorkerErrors.WorkerWithIdDoesnotExist;

        return worker;
    }
}