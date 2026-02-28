using Nexticz.Module.Sign.SharedKernel;

namespace Nexticz.Module.Sign.Settings.Presentation;

internal static class SettingsEndpoints
{
    private const string SettingsBase = ApiEndpoints.ApiBase + "/settings";
    public static string GetOpenApiName(string sectionName) => SettingsBase + sectionName;

    internal static class OpenApiContractEndpoints
    {
        private const string Base = $"{SettingsBase}/openapicontracts";
        
        public const string GetOpenApiContracts = $"{Base}";
    }
    
    internal static class ImportEndpoints
    {
        private const string Base = $"{SettingsBase}/imports";
        
        public const string CreateImport = $"{Base}";
    }
    
    internal static class ExportEndpoints
    {
        private const string Base = $"{SettingsBase}/exports";
        
        public const string CreateExport = $"{Base}";
    }
    
    internal static class HistoryEventEndpoints
    {
        public const string Base = $"{SettingsBase}/historyevents";
        
        public const string GetHistoryEventsByStreamId = $"{Base}/{{streamId}}";
    }
    
    internal static class DepositorGroupEndpoints
    {
        private const string Base = $"{SettingsBase}/depositorgroups";
        
        public const string CreateDepositorGroup = $"{Base}";
        public const string GetDepositorGroupByCode = $"{Base}/{{code}}";
        public const string GetDepositorGroups = $"{Base}";
        public const string UpdateDepositorGroup = $"{Base}/{{code}}";
        public const string DeleteDepositorGroup = $"{Base}/{{code}}";
    }
    
    internal static class DeliveryMethodEndpoints
    {
        private const string Base = $"{SettingsBase}/deliverymethods";
        
        public const string CreateDeliveryMethod = $"{Base}";
        public const string GetDeliveryMethodByCode = $"{Base}/{{code}}";
        public const string GetDeliveryMethods = $"{Base}";
        public const string UpdateDeliveryMethod = $"{Base}/{{code}}";
        public const string DeleteDeliveryMethod = $"{Base}/{{code}}";
    }
    
    internal static class PartnerEndpoints
    {
        private const string Base = $"{SettingsBase}/partners";
        
        public const string CreatePartner = $"{Base}";
        public const string GetPartnerByCode = $"{Base}/{{code}}";
        public const string GetPartners = $"{Base}";
        public const string UpdatePartner = $"{Base}/{{code}}";
        public const string DeletePartner = $"{Base}/{{code}}";
    }
    
    internal static class LocationEndpoints
    {
        private const string Base = $"{SettingsBase}/locations";
        
        public const string CreateLocation = $"{Base}";
        public const string GetLocationByCode = $"{Base}/{{code}}";
        public const string GetLocations = $"{Base}";
        public const string UpdateLocation = $"{Base}/{{code}}";
        public const string DeleteLocation = $"{Base}/{{code}}";
    }
    
    internal static class PrinterEndpoints
    {
        private const string Base = $"{SettingsBase}/printers";
        
        public const string CreatePrinter = $"{Base}";
        public const string GetPrinterByCode = $"{Base}/{{code}}";
        public const string GetPrinters = $"{Base}";
        public const string UpdatePrinter = $"{Base}/{{code}}";
        public const string DeletePrinter = $"{Base}/{{code}}";
    }
    
    internal static class SigningDeviceEndpoints
    {
        private const string Base = $"{SettingsBase}/signingdevices";
        
        public const string CreateSigningDevice = $"{Base}";
        public const string GetSigningDeviceByCode = $"{Base}/{{code}}";
        public const string GetSigningDevices = $"{Base}";
        public const string UpdateSigningDevice = $"{Base}/{{code}}";
        public const string DeleteSigningDevice = $"{Base}/{{code}}";
    }
    
    internal static class DepositorEndpoints
    {
        private const string Base = $"{SettingsBase}/depositors";
        
        public const string CreateDepositor = $"{Base}";
        public const string GetDepositorByCode = $"{Base}/{{code}}";
        public const string GetDepositors = $"{Base}";
        public const string UpdateDepositor = $"{Base}/{{code}}";
        public const string DeleteDepositor = $"{Base}/{{code}}";
    }
    
    internal static class ReceiverEndpoints
    {
        private const string Base = $"{SettingsBase}/receivers";
        
        public const string CreateReceiver = $"{Base}";
        public const string GetReceiverByCodeAndPartnerCode = $"{Base}/{{code}}/{{partnerCode}}";
        public const string GetReceivers = $"{Base}";
        public const string UpdateReceiver = $"{Base}/{{code}}/{{partnerCode}}";
        public const string DeleteReceiver = $"{Base}/{{code}}/{{partnerCode}}";
    }
    
    internal static class UserEndpoints
    {
        private const string Base = $"{SettingsBase}/users";
        
        public const string GetUsers = $"{Base}";
        public const string UpdateUser = $"{Base}/{{username}}";
        public const string UploadSignature = $"{Base}/{{username}}/uploadedsignatures";
        public const string DeleteSignature = $"{Base}/{{username}}/deletedsignatures";
        public const string GetSignature = $"{Base}/{{username}}/usersignatures";
    }
    
    internal static class EmailConfigurationEndpoints
    {
        private const string Base = $"{SettingsBase}/emailconfigurations";
        
        public const string GetEmailConfigurations = $"{Base}";
        public const string GetEmailConfigurationById = $"{Base}/{{id}}";
        public const string UpdateEmailConfiguration = $"{Base}/{{id}}";
        public const string DeleteEmailConfiguration = $"{Base}/{{id}}";
        public const string CreateEmailConfiguration = $"{Base}";
    }
    
    internal static class DocumentTemplateEndpoints
    {
        private const string Base = $"{SettingsBase}/DocumentTemplates";
        
        public const string GetDocumentTemplates = $"{Base}";
        public const string GetDocumentTemplateByCode = $"{Base}/{{code}}";
        public const string UpdateDocumentTemplate = $"{Base}/{{code}}";
        public const string DeleteDocumentTemplate = $"{Base}/{{code}}";
        public const string CreateDocumentTemplate = $"{Base}";
    }
    
    internal static class EmailTemplateEndpoints
    {
        private const string Base = $"{SettingsBase}/emailtemplates";
        
        public const string GetEmailTemplates = $"{Base}";
        public const string GetEmailTemplateById = $"{Base}/{{code}}";
        public const string UpdateEmailTemplate = $"{Base}/{{code}}";
        public const string DeleteEmailTemplate = $"{Base}/{{code}}";
        public const string CreateEmailTemplate = $"{Base}";
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
}