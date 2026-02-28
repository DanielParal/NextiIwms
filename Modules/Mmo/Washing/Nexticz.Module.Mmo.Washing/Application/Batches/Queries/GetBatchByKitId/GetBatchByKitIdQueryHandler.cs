using MediatR;
using ErrorOr;
using Nexticz.Module.Mmo.Washing.Application.Interfaces;
using Nexticz.Module.Mmo.Washing.Domain.BatchAggregate;

namespace Nexticz.Module.Mmo.Washing.Application.Batches.Queries.GetBatchByKitId;

internal class GetBatchByKitIdQueryHandler(IBatchReadOnlyRepository batchReadOnlyRepository)
    : IRequestHandler<GetBatchByKitIdQuery, ErrorOr<Batch>>
{
    public async Task<ErrorOr<Batch>> Handle(GetBatchByKitIdQuery request, CancellationToken cancellationToken)
    {
        var batch = await batchReadOnlyRepository.GetBatchByKitIdAsync(request.KitId, cancellationToken);
        if (batch is null)
            return BatchErrors.BatchNotFound;

        return batch;
    }
}