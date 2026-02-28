using Nexticz.Module.Mmo.SharedKernel;

namespace Nexticz.Module.Mmo.Drying.Presentation;

internal static class DryingEndpoints
{
    private const string DryingBase = ApiEndpoints.ApiBase + "/drying";
    public static string GetOpenApiName(string sectionName) => DryingBase + sectionName;
    
    internal static class KitEndpoints
    {
        private const string Base = $"{DryingBase}/kits";
        
        public const string GetKits = Base;
        public const string FinishKit = $"{Base}/{{kitId}}/driedkits";
        public const string TransferKit = $"{Base}/{{kitId}}/transferredkits";
        public const string GetKitByCompletedKitsCount = $"{Base}/{{completedKitsCount}}";
    }
}