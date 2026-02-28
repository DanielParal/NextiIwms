namespace Nexticz.Module.Cuzk.Presentation;

internal static class CuzkEndpoints
{
    private const string CuzkBase = "/api/cuzk";
    public static string GetOpenApiName(string sectionName) => CuzkBase + sectionName;
    
    internal static class MunicipalityEndpoints
    {
        private const string Base = $"{CuzkBase}/municipalities";
        
        public const string CreateMunicipality = $"{Base}";
        public const string GetMunicipalityByCode = $"{Base}/{{code}}";
        public const string GetMunicipalities = $"{Base}";
        public const string UpdateMunicipality = $"{Base}/{{code}}";
        public const string DeleteMunicipality = $"{Base}/{{code}}";
    }
    
    internal static class ImportEndpoints
    {
        private const string Base = $"{CuzkBase}/imports";
        
        public const string RequestImport = $"{Base}";
        public const string GetImports = $"{Base}";
    }
    
    internal static class AddressLocationEndpoints
    {
        private const string Base = $"{CuzkBase}/addressLocations";
        
        public const string CreateAddressLocation = $"{Base}";
        public const string GetAddressLocationByCode = $"{Base}/{{code}}";
        public const string GetAddressLocations = $"{Base}";
        public const string UpdateAddressLocation = $"{Base}/{{code}}";
        public const string DeleteAddressLocation = $"{Base}/{{code}}";
    }
    
    internal static class AddressLocationSlugEndpoints
    {
        private const string Base = $"{CuzkBase}/addressLocationSlugs";
        
        public const string Get = $"{Base}/{{searchQuery}}/{{take?}}";
    }
    
    internal static class OpenApiContractEndpoints
    {
        private const string Base = $"{CuzkBase}/openapicontracts";
        
        public const string GetOpenApiContracts = $"{Base}";
    }
    
    internal static class EconomicSubjectEndpoints
    {
        private const string Base = $"{CuzkBase}/economicSubjects";
        
        public const string Get = $"{Base}/{{ico}}";
    }

}