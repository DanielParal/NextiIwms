using Nexticz.Module.Sign.SharedKernel;

namespace Nexticz.Module.Sign.DocumentManager.Presentation;

internal static class DocumentManagerEndpoints
{
    private const string DocumentManagerBase = ApiEndpoints.ApiBase + "/documentmanager";
    public static string GetOpenApiName(string sectionName) => DocumentManagerBase + sectionName;
    
    internal static class UnsignedLoadingDocumentEndpoints
    {
        private const string Base = $"{DocumentManagerBase}/unsignedloadingdocuments";
        
        public const string GetUnsignedLoadingDocuments = $"{Base}";
    }
    
    internal static class SignedLoadingDocumentEndpoints
    {
        private const string Base = $"{DocumentManagerBase}/signedloadingdocuments";
        
        public const string GetSignedLoadingDocuments = $"{Base}";
    }
    
    internal static class LoadingDocumentEndpoints
    {
        private const string Base = $"{DocumentManagerBase}/loadingdocuments";
        
        public const string ManuallySignDocument = $"{Base}/{{loadingDocumentCode}}/manuallysigned";
        public const string ChangePrintCopiesCount = $"{Base}/{{loadingDocumentCode}}/printcopieschanged";
        public const string DownloadSignedDocuments = $"{Base}/DownloadedDocuments";
        public const string DeleteDocuments = $"{Base}/DeletedDocuments";
    }
    
    internal static class SigningDeviceEndpoints
    {
        private const string Base = $"{DocumentManagerBase}/signingdevices";
        
        public const string GetSigningDevices = $"{Base}";
        public const string SendLoadingDocumentToSigningDevice = $"{Base}/{{signingDeviceCode}}/SentLoadingDocuments";
        public const string ReturnLoadingDocumentFromSigningDevice = $"{Base}/{{signingDeviceCode}}/ReturnedLoadingDocuments";
        public const string GetDocumentsForSigningDevice = $"{Base}/{{signingDeviceCode}}/documents";
        public const string GetDocumentFileForSigningDevice = $"{Base}/{{signingDeviceCode}}/files/{{documentCode}}";
        public const string SignDocuments = $"{Base}/{{signingDeviceCode}}/SignedDocuments";
    }
    
    internal static class EmailEndpoints
    {
        private const string Base = $"{DocumentManagerBase}/emails";
        
        public const string GetEmails = $"{Base}";
        public const string SendEmails = $"{Base}";
    }
    
    internal static class ProjectionEndpoints
    {
        private const string Base = $"{DocumentManagerBase}/projections";
        
        public const string RebuildProjection = $"{Base}/rebuild/{{projectionTypeString}}";
    }
}