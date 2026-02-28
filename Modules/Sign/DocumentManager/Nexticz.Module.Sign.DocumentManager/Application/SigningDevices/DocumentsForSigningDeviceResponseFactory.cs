using Nexticz.Module.Sign.DocumentManager.Contracts.SigningDevices;
using Nexticz.Module.Sign.DocumentManager.Domain.SigningDeviceAggregate;

namespace Nexticz.Module.Sign.DocumentManager.Application.SigningDevices;

internal static class DocumentsForSigningDeviceResponseFactory
{
    public static DocumentsForSigningDeviceResponse Create(SentLoadingDocument[] sentLoadingDocuments)
    {
        var licensePlate = GetCommonLicensePlateForAllLoadingDocuments(sentLoadingDocuments);
        var driverName = GetCommonDriverNameForAllLoadingDocuments(sentLoadingDocuments);
        var totalWeight = GetTotalWeight(sentLoadingDocuments);
        var totalAdrPoints = GetTotalAdrPoints(sentLoadingDocuments);
        
        var loadingDocumentsToSign = 
            sentLoadingDocuments
                .Select(x => 
                    new LoadingDocumentToSignContract(
                        x.LoadingDocumentCode,
                        x.DeliveryDocuments.Select(dd => new DeliveryDocumentToSignContract(dd.Code, dd.PartnersOrderNumber)).ToArray(),
                        x.ShouldAlsoSendLoadingDocument))
                .ToArray();
        
        return new DocumentsForSigningDeviceResponse(licensePlate, driverName, totalWeight, totalAdrPoints, loadingDocumentsToSign);
    }
    
    private static string? GetCommonLicensePlateForAllLoadingDocuments(SentLoadingDocument[] sentLoadingDocuments)
    {
        var licensePlates = sentLoadingDocuments.Select(x => x.LicensePlate).Distinct().ToArray();
        return licensePlates.Length == 1 ? licensePlates[0] : null;
    }
    
    private static string? GetCommonDriverNameForAllLoadingDocuments(SentLoadingDocument[] sentLoadingDocuments)
    {
        var driverNames = sentLoadingDocuments.Select(x => x.DriverName).Distinct().ToArray();
        return driverNames.Length == 1 ? driverNames[0] : null;
    }

    private static decimal? GetTotalWeight(SentLoadingDocument[] sentLoadingDocuments)
    {
        var loadingDocuments = sentLoadingDocuments.Where(x => x.ShouldAlsoSendLoadingDocument).ToArray();
        return loadingDocuments.Length == 0 ? null : loadingDocuments.Sum(x => x.TotalWeight ?? 0m);
    }
    
    private static int? GetTotalAdrPoints(SentLoadingDocument[] sentLoadingDocuments)
    {
        var loadingDocuments = sentLoadingDocuments.Where(x => x.ShouldAlsoSendLoadingDocument).ToArray();
        return loadingDocuments.Length == 0 ? null : loadingDocuments.Sum(x => x.TotalAdrPoints ?? 0);
    }
}