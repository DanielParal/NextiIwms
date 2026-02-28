using Nexticz.Module.Mmo.SharedKernel;

namespace Nexticz.Module.Mmo.Washing.Presentation;

internal static class WashingEndpoints
{
    private const string WashingBase = ApiEndpoints.ApiBase + "/washing";
    public static string GetOpenApiName(string sectionName) => WashingBase + sectionName;

    internal static class BatchesEndpoints
    {
        private const string Base = $"{WashingBase}/batches";
        
        public const string FinishKit = $"{Base}/{{batchId}}/finishedkits";
        public const string GetBatchByLineCode = $"{Base}";
        public const string CreatePrinting = $"{Base}/{{batchId}}/kitprintings";
        public const string GetKitInstructionFile = $"{Base}/{{batchId}}/kitinstructions";
        public const string ConfirmSpecialInformation = $"{Base}/{{batchId}}/confirmedspecialinformations";
        public const string GetSpecialInformationFile = $"{Base}/{{batchId}}/specialinformationfiles";
    }
    
    internal static class WashingMachineSosEndpoints
    {
        private const string Base = $"{WashingBase}/sos";
        
        public const string CallSos = $"{Base}/{{washingMachineCode}}/calledsoses";
        public const string ResolveSos = $"{Base}/{{washingMachineCode}}/resolvedsoses";
    }
    
    internal static class LastEnteredWorkerOnLineEndpoints
    {
        private const string Base = $"{WashingBase}/lines";
        
        public const string EnterLine = $"{Base}/{{lineCode}}/enteredworkers";
        public const string LeaveLine = $"{Base}/{{lineCode}}/leftworkers";
    }
    
    internal static class WashingStateEndpoints
    {
        private const string Base = $"{WashingBase}/washingstates";
        
        public const string GetWashingState = $"{Base}";
    }
    
    internal static class SimulationEndpoints
    {
        private const string Base = $"{WashingBase}/simulations";
        
        public const string StartSimulation = $"{Base}/start";
        public const string StopSimulation = $"{Base}/stop";
    }
}