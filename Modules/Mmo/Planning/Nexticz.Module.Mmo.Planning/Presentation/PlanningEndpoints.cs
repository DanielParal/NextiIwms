using Nexticz.Module.Mmo.SharedKernel;

namespace Nexticz.Module.Mmo.Planning.Presentation;

internal static class PlanningEndpoints
{
    private const string PlanningBase = ApiEndpoints.ApiBase + "/planning";
    public static string GetOpenApiName(string sectionName) => PlanningBase + sectionName;
    
    internal static class WashingMachineEndpoints
    {
        private const string Base = $"{PlanningBase}/washingmachines/{{washingMachineCode}}";
        
        public const string GetWashingMachines = $"{PlanningBase}/washingmachines";
        public const string CreateBatch = $"{Base}/batches";
        public const string RemoveBatch = $"{Base}/batches/{{batchId}}";
        public const string ChangeBatchKitsCount = $"{Base}/batches/{{batchId}}/kitscountchanges";
        public const string MoveBatchInQueue = $"{Base}/batches/{{batchId}}/positionmovings";
        public const string MoveBatchToAnotherQueue = $"{Base}/batches/{{batchId}}/queuemovings";
        public const string ActivateBatch = $"{Base}/batches/{{batchId}}/batchactivations";
        public const string FinishBatch = $"{Base}/batches/{{batchId}}/finishedbatches";
        public const string SplitBatch = $"{Base}/batches/{{batchId}}/splitbatches";
        public const string DetachSisterBatch = $"{Base}/batches/{{batchId}}/detachedbatches";
    }
}