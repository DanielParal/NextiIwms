using Nexticz.Lib.Shared.FileHandling.Assets;

namespace Nexticz.Module.Sign.SharedKernel.FileHandling;

public static class DirectoryNamesProvider
{
    private const string ModuleName = "SIGN";
    
    public static string GetBaseSignaturePath(AssetsSettings assetsSettings) => $"{assetsSettings.BaseFolder}/{ModuleName}/UserSignatures";
    public static string GetUserSignatureFileNameWithoutExtension => "SignatureWithStamp";
    public static string GetBaseLoaderPath(AssetsSettings assetsSettings) => $"{assetsSettings.BaseFolder}/{ModuleName}/DocumentLoader";
    public static string GetLoadingListExtension => " - Loading list.xml";
    public static string GetLoadingListXmlFileName(string loadingListCode) => $"{loadingListCode}{GetLoadingListExtension}";
    public static string GetLoadingListPdfFileName(string loadingListCode) => $"{loadingListCode}.pdf";
    public static string GetDeliveryNoteXmlFileName(string loadingListCode, string deliveryNoteCode) => $"{loadingListCode} - Delivery note no. {deliveryNoteCode}.xml";
    public static string GetDeliveryNotePdfFileName(string deliveryNoteCode) => $"{deliveryNoteCode}.pdf";
    public static string GetBaseManagerPath(AssetsSettings assetsSettings) => $"{assetsSettings.BaseFolder}/{ModuleName}/DocumentManager";
    public static string GetBaseHistoryPath(AssetsSettings assetsSettings) => $"{assetsSettings.BaseFolder}/{ModuleName}/DocumentHistory";
    public static string GetHistoryLoadingDocumentFilePath(AssetsSettings assetsSettings, string loadingDocumentCode) 
        => $"{GetBaseHistoryPath(assetsSettings)}/{loadingDocumentCode}/{loadingDocumentCode}.pdf";
    public static string GetHistoryDeliveryDocumentFilePath(AssetsSettings assetsSettings, string loadingDocumentCode, string deliveryDocumentCode) 
        => $"{GetBaseHistoryPath(assetsSettings)}/{loadingDocumentCode}/{deliveryDocumentCode}.pdf";
    public static string GetHistoryLoadingDocumentOriginalFilesPath(AssetsSettings assetsSettings, string loadingDocumentCode) => $"{GetBaseHistoryPath(assetsSettings)}/{loadingDocumentCode}/OriginalFiles";
    public static string GetHistoryPrintingsFolder(AssetsSettings assetsSettings) => $"{assetsSettings.BaseFolder}/{ModuleName}/DocumentHistory/Printings";
}