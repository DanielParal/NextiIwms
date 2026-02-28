using Nexticz.Module.Mmo.SharedKernel;

namespace Nexticz.Module.Mmo.Settings.Presentation;

internal static class SettingsEndpoints
{
    private const string SettingsBase = ApiEndpoints.ApiBase + "/settings";
    public static string GetOpenApiName(string sectionName) => SettingsBase + sectionName;
    
    internal static class OpenApiContractEndpoints
    {
        private const string Base = $"{SettingsBase}/openapicontracts";
        
        public const string GetOpenApiContracts = $"{Base}";
    }
    
    internal static class KitEndpoints
    {
        private const string Base = $"{SettingsBase}/kits";
        
        public const string GetKitByCode = $"{Base}/{{code}}";
        public const string GetKits = $"{Base}";
        public const string CreateKit = $"{Base}";
        public const string UpdateKit = $"{Base}/{{code}}";
        public const string DeleteKit = $"{Base}/{{code}}";
        public const string UploadKitInstruction = $"{Base}/{{code}}/uploadedkitinstructions";
        public const string DeleteKitInstruction = $"{Base}/{{code}}/uploadedkitinstructions";
        public const string GetKitInstructionFile = $"{Base}/{{code}}/kitinstructions";
        public const string GetKitSpecialInformations = $"{Base}/{{code}}/specialinformations";
    }
    
    internal static class KitTypeEndpoints
    {
        private const string Base = $"{SettingsBase}/kittypes";
        
        public const string GetKitTypeByCode = $"{Base}/{{code}}";
        public const string GetKitTypes = $"{Base}";
        public const string CreateKitType = $"{Base}";
        public const string UpdateKitType = $"{Base}/{{code}}";
        public const string DeleteKitType = $"{Base}/{{code}}";
    }
    
    internal static class KitSapDefinitionEndpoints
    {
        private const string Base = $"{SettingsBase}/kitsapdefinitions";
        
        public const string GetKitSapDefinitionByCode = $"{Base}/{{code}}";
        public const string GetKitSapDefinitions = $"{Base}";
        public const string CreateKitSapDefinition = $"{Base}";
        public const string UpdateKitSapDefinition = $"{Base}/{{code}}";
        public const string DeleteKitSapDefinition = $"{Base}/{{code}}";
    }
    
    internal static class DepositorEndpoints
    {
        private const string Base = $"{SettingsBase}/depositors";
        
        public const string GetDepositorByCode = $"{Base}/{{code}}";
        public const string GetDepositors = $"{Base}";
        public const string CreateDepositor = $"{Base}";
        public const string UpdateDepositor = $"{Base}/{{code}}";
        public const string DeleteDepositor = $"{Base}/{{code}}";
    }
    
    internal static class PackagingTypeEndpoints
    {
        private const string Base = $"{SettingsBase}/packagingtypes";
        
        public const string GetPackagingTypeByCode = $"{Base}/{{code}}";
        public const string GetPackagingTypes = $"{Base}";
        public const string CreatePackagingType = $"{Base}";
        public const string UpdatePackagingType = $"{Base}/{{code}}";
        public const string DeletePackagingType = $"{Base}/{{code}}";
    }
    
    internal static class PackagingEndpoints
    {
        private const string Base = $"{SettingsBase}/packagings";
        
        public const string GetPackagingByCode = $"{Base}/{{code}}";
        public const string GetPackagings = $"{Base}";
        public const string CreatePackaging = $"{Base}";
        public const string UpdatePackaging = $"{Base}/{{code}}";
        public const string DeletePackaging = $"{Base}/{{code}}";
    }
    
    internal static class ManufactureEndpoints
    {
        private const string Base = $"{SettingsBase}/manufactures";
        
        public const string GetManufactureByCode = $"{Base}/{{code}}";
        public const string GetManufactures = $"{Base}";
        public const string CreateManufacture = $"{Base}";
        public const string UpdateManufacture = $"{Base}/{{code}}";
        public const string DeleteManufacture = $"{Base}/{{code}}";
    }
    
    internal static class PackagingCirculationEndpoints
    {
        private const string Base = $"{SettingsBase}/packagingcirculations";
        
        public const string GetPackagingCirculationByCode = $"{Base}/{{code}}";
        public const string GetPackagingCirculations = $"{Base}";
        public const string CreatePackagingCirculation = $"{Base}";
        public const string UpdatePackagingCirculation = $"{Base}/{{code}}";
        public const string DeletePackagingCirculation = $"{Base}/{{code}}";
    }
    
    internal static class WashingMachineEndpoints
    {
        private const string Base = $"{SettingsBase}/washingmachines";
        
        public const string GetWashingMachineByCode = $"{Base}/{{code}}";
        public const string GetWashingMachines = $"{Base}";
        public const string CreateWashingMachine = $"{Base}";
        public const string UpdateWashingMachine = $"{Base}/{{code}}";
    }
    
    internal static class ImportEndpoints
    {
        private const string Base = $"{SettingsBase}/imports";
        
        public const string GetImports = $"{Base}";
        public const string CreateImport = $"{Base}";
    }
    
    internal static class ExportEndpoints
    {
        private const string Base = $"{SettingsBase}/exports";
        
        public const string GetExports = $"{Base}";
        public const string CreateExport = $"{Base}";
    }
    
    internal static class HistoryEventEndpoints
    {
        public const string Base = $"{SettingsBase}/historyevents";
        
        public const string GetHistoryEventsByStreamId = $"{Base}/{{streamId}}";
    }
    
    internal static class SeedEndpoints
    {
        private const string Base = $"{SettingsBase}/seeds";
        
        public const string CreateSeed = $"{Base}";
    }
    
    internal static class ProjectionEndpoints
    {
        private const string Base = $"{SettingsBase}/projections";
        
        public const string RebuildProjection = $"{Base}/rebuild/{{projectionTypeString}}";
    }
    
    internal static class ConstantEndpoints
    {
        private const string Base = $"{SettingsBase}/constants";
        
        public const string GetConstantByKey = $"{Base}/{{key}}";
        public const string GetConstants = $"{Base}";
        public const string CreateConstant = $"{Base}";
        public const string UpdateConstant = $"{Base}/{{key}}";
        public const string DeleteConstant = $"{Base}/{{key}}";
    }
    
    internal static class WorkerEndpoints
    {
        private const string Base = $"{SettingsBase}/workers";
        
        public const string GetWorkerByPin = $"{Base}/{{pin}}";
        public const string GetWorkers = $"{Base}";
        public const string CreateWorker = $"{Base}";
        public const string UpdateWorker = $"{Base}/{{id}}";
        public const string DeleteWorker = $"{Base}/{{id}}";
    }
    
    internal static class InactivityTypeEndpoints
    {
        private const string Base = $"{SettingsBase}/inactivitytypes";
        
        public const string GetInactivityTypeById = $"{Base}/{{id}}";
        public const string GetInactivityTypes = $"{Base}";
        public const string CreateInactivityType = $"{Base}";
        public const string UpdateInactivityType = $"{Base}/{{id}}";
        public const string DeleteInactivityType = $"{Base}/{{id}}";
    }
    
    internal static class UserEndpoints
    {
        private const string Base = $"{SettingsBase}/users";
        
        public const string GetUsers = $"{Base}";
        public const string UpdateUser = $"{Base}/{{id}}";
    }
    
    internal static class SpecialInformationEndpoints
    {
        private const string Base = $"{SettingsBase}/specialinformations";
        
        public const string GetSpecialInformations = $"{Base}";
        public const string GetSpecialInformationById = $"{Base}/{{id}}";
        public const string CreateSpecialInformation = $"{Base}";
        public const string UpdateSpecialInformation = $"{Base}/{{id}}";
        public const string DeleteSpecialInformation = $"{Base}/{{id}}";
        public const string UploadFile = $"{Base}/{{id}}/files";
        public const string DeleteFile = $"{Base}/{{id}}/files";
        public const string GetFile = $"{Base}/{{id}}/files";
    }
}