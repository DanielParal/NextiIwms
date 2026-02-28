using ErrorOr;
using MediatR;
using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Domain.WorkerEntity;

namespace Nexticz.Module.Mmo.Settings.Application.Workers.Queries.GetWorkerById;

internal class GetWorkerByIdQueryHandler(ISettingsReadOnlyEventStoreRepository readOnlyRepository) 
    : IRequestHandler<GetWorkerByIdQuery, ErrorOr<Worker>>
{
    public async Task<ErrorOr<Worker>> Handle(GetWorkerByIdQuery request, CancellationToken cancellationToken)
    {
        var kitCompleter = await readOnlyRepository.GetByIdAsync<Worker>(request.Id, cancellationToken);
        
        if (kitCompleter is null)
            return WorkerErrors.NotFoundWorkerWithId;
        
        return kitCompleter;
    }
}