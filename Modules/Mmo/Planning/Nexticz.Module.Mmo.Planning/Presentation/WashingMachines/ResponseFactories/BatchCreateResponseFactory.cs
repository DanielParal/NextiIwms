using Nexticz.Module.Mmo.Planning.Contracts.WashingMachines;
using Nexticz.Module.Mmo.Planning.Application.WashingMachines;
using Nexticz.Module.Mmo.Planning.Domain.BatchEntity;

namespace Nexticz.Module.Mmo.Planning.Presentation.WashingMachines.ResponseFactories;

internal class BatchCreateResponseFactory
{
    public static CreateBatchResponse Create(Batch batch)
    {
        var batchResponse = BatchContractFactory.Create(batch);
        return new CreateBatchResponse(batchResponse, null);
    }
    
    public static CreateBatchResponse Create(Batch batch, Batch sisterBatch)
    {
        var batchResponse = BatchContractFactory.Create(batch);
        var sisterBatchResponse = BatchContractFactory.Create(sisterBatch);
        return new CreateBatchResponse(batchResponse, sisterBatchResponse);
    }
}