using MediatR;
using Nexticz.Module.Mmo.Washing.Application.Interfaces;
using Nexticz.Module.Mmo.Washing.Domain.BatchAggregate;

namespace Nexticz.Module.Mmo.Washing.Application.Batches.Queries.GetBatchesByWashingMachineCode;

internal class GetBatchesByWashingMachineCodeQueryHandler(IBatchReadOnlyRepository batchReadOnlyRepository) : IRequestHandler<GetBatchesByWashingMachineCodeQuery, Batch[]>
{
    public async Task<Batch[]> Handle(GetBatchesByWashingMachineCodeQuery request, CancellationToken cancellationToken)
    {
        var batches = await batchReadOnlyRepository.GetBatchesByWashingMachineCodeAsync(request.WashingMachineCode, cancellationToken);
        return batches.ToArray();
    }
}