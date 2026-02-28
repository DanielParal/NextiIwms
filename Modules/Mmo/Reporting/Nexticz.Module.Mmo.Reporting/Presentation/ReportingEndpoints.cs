using Nexticz.Module.Mmo.SharedKernel;

namespace Nexticz.Module.Mmo.Reporting.Presentation;

internal static class ReportingEndpoints
{
    private const string ReportingBase = ApiEndpoints.ApiBase + "/reporting";
    public static string GetOpenApiName(string sectionName) => ReportingBase + sectionName;
    
    internal static class DriedKitEndpoints
    {
        private const string Base = $"{ReportingBase}/driedkits";
        
        public const string GetDriedKits = $"{Base}";
    }
    
    internal static class ShiftSettingEndpoints
    {
        private const string Base = $"{ReportingBase}/shiftsettings";
        
        public const string GetShiftSettings = $"{Base}";
    }
    
    internal static class ShiftEndpoints
    {
        private const string Base = $"{ReportingBase}/shifts";
        
        public const string GetShiftDetail = $"{ReportingBase}/{{shiftId}}/shiftsdetails";
        public const string GetShiftsSummaries = $"{ReportingBase}/shiftssummaries";
        public const string GetShiftSelections = $"{ReportingBase}/shiftSelections";
        public const string ApproveShift = $"{ReportingBase}/{{shiftId}}/approvedShifts";
    }
    
    internal static class LineItemEndpoints
    {
        private const string Base = $"{ReportingBase}/lineitems";
        
        public const string ChangeComment = $"{Base}/{{id}}/comments";
        public const string ChangeItem = $"{Base}/{{id}}/changeditems";
        public const string AddItems = $"{Base}";
    }
}