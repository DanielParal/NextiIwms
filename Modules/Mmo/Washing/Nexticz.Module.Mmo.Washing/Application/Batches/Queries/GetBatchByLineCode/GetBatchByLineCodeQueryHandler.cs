using MediatR;
using Nexticz.Module.Mmo.Washing.Application.Interfaces;
using Nexticz.Module.Mmo.Washing.Domain.BatchAggregate;

namespace Nexticz.Module.Mmo.Washing.Application.Batches.Queries.GetBatchByLineCode;

internal class GetBatchByLineCodeQueryHandler(
    IBatchReadOnlyRepository batchReadOnlyRepository
    ) : IRequestHandler<GetBatchByLineCodeQuery, Batch?>
{
    public async Task<Batch?> Handle(GetBatchByLineCodeQuery request, CancellationToken cancellationToken)
    {
        return await batchReadOnlyRepository.GetBatchByLineCodeAsync(request.LineCode, cancellationToken);
    }
}