using MediatR;
using ErrorOr;
using Nexticz.Module.Mmo.Washing.Application.Interfaces;
using Nexticz.Module.Mmo.Washing.Domain.BatchAggregate;

namespace Nexticz.Module.Mmo.Washing.Application.Batches.Queries.GetBatchById;

internal class GetBatchByIdQueryHandler(
    IWashingReadOnlyEventStoreRepository washingReadOnlyEventStoreRepository) : IRequestHandler<GetBatchByIdQuery, ErrorOr<Batch>>
{
    public async Task<ErrorOr<Batch>> Handle(GetBatchByIdQuery request, CancellationToken cancellationToken)
    {
        var batch = await washingReadOnlyEventStoreRepository.GetByIdAsync<Batch>(request.Id, cancellationToken);

        if (batch is null)
            return BatchErrors.BatchNotFound;

        return batch;
    }
}