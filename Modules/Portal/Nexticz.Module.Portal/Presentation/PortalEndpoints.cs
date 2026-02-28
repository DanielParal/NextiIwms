namespace Nexticz.Module.Portal.Presentation;

internal static class PortalEndpoints
{
    private const string SettingsBase = "/api/portal";
    public static string GetOpenApiName(string sectionName) => SettingsBase + sectionName;
    
    internal static class ModuleEndpoints
    {
        private const string Base = $"{SettingsBase}/modules";
        
        public const string CreateModule = $"{Base}";
        public const string GetModuleById = $"{Base}/{{id}}";
        public const string GetModules = $"{Base}";
        public const string UpdateModule = $"{Base}/{{id}}";
        public const string DeleteModule = $"{Base}/{{id}}";
        public const string ChangeModuleOrder = $"{Base}/{{id}}/orderChanges";
    }
    
    internal static class SeedEndpoints
    {
        private const string Base = $"{SettingsBase}/seeds";
        
        public const string CreateSeed = $"{Base}";
    }

}