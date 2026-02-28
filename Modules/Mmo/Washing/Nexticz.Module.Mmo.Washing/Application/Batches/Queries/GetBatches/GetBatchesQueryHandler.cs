using MediatR;
using Nexticz.Module.Mmo.Washing.Application.Interfaces;
using Nexticz.Module.Mmo.Washing.Domain.BatchAggregate;

namespace Nexticz.Module.Mmo.Washing.Application.Batches.Queries.GetBatches;

internal class GetBatchesQueryHandler(
    IWashingReadOnlyEventStoreRepository washingReadOnlyEventStoreRepository) 
    : IRequestHandler<GetBatchesQuery, Batch[]>
{
    public async Task<Batch[]> Handle(GetBatchesQuery request, CancellationToken cancellationToken)
    {
        var batches = await washingReadOnlyEventStoreRepository.GetAllAsync<Batch>(cancellationToken);
        return batches.ToArray();
    }
}