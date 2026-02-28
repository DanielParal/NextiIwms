using ErrorOr;
using MediatR;
using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Domain.WorkerEntity;

namespace Nexticz.Module.Mmo.Settings.Application.Workers.Queries.GetWorkerByPin;

internal class GetWorkerByPinQueryHandler(IWorkerReadOnlyRepository readOnlyRepository) 
    : IRequestHandler<GetWorkerByPinQuery, ErrorOr<Worker>>
{
    public async Task<ErrorOr<Worker>> Handle(GetWorkerByPinQuery request, CancellationToken cancellationToken)
    {
        var kitCompleter = await readOnlyRepository.GetByPinAsync(request.Pin, cancellationToken);

        if (kitCompleter is null)
            return WorkerErrors.NotFoundWorkerWithPin;
        
        return kitCompleter;
    }
}