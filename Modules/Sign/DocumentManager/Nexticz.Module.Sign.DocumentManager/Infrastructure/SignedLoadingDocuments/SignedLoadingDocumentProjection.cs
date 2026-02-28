using Marten.Events.Projections;
using Nexticz.Module.Sign.DocumentManager.Domain.LoadingDocumentAggregate;
using Nexticz.Module.Sign.DocumentManager.Domain.LoadingDocumentAggregate.Events;
using Nexticz.Module.Sign.DocumentManager.Domain.Views;

namespace Nexticz.Module.Sign.DocumentManager.Infrastructure.SignedLoadingDocuments;

public class SignedLoadingDocumentProjection : MultiStreamProjection<SignedLoadingDocumentView, Guid>
{
    public SignedLoadingDocumentProjection()
    {
        Identity<SignedDocumentManuallyUploadedEvent>(x => GenerateProjectionId(x.Id));
        Identity<LoadingDocumentSignedEvent>(x => GenerateProjectionId(x.Id));
        Identity<LoadingDocumentWithAllDeliveryDocumentsFinishedEvent>(x => GenerateProjectionId(x.Id));
        Identity<LoadingDocumentDeletedEvent>(x => GenerateProjectionId(x.Id));
        
        ProjectEventAsync<SignedDocumentManuallyUploadedEvent>(async (session, docView, currentEvent) =>
        {
            var loadingDocument = await session.LoadAsync<LoadingDocument>(currentEvent.Id);

            if (loadingDocument is null)
                return;
            
            MapLoadingDocumentView(docView, loadingDocument, currentEvent.IsLoadingDocumentSignature, currentEvent.UploadedByUserName, currentEvent.UploadedByUserFullName,
                currentEvent.UploadedAt, FinishMethod.ManuallyUploaded, 0, docView.DriverName, docView.LicensePlate, docView.DeletionReason);

            if (currentEvent.IsLoadingDocumentSignature)
                return;
            
            var deliveryDocument = loadingDocument.DeliveryDocuments.First(x => x.Code == currentEvent.DeliveryDocumentCode);
            
            docView.DeliveryDocuments.Add(
                new SignedDeliveryDocument(
                    deliveryDocument.Code,
                    deliveryDocument.LoadingDocumentCode,
                    deliveryDocument.PartnerCode,
                    deliveryDocument.PartnerNameShort,
                    deliveryDocument.DeliveryMethodCode,
                    deliveryDocument.DeliveryMethodName,
                    deliveryDocument.SignedWithLicensePlate,
                    deliveryDocument.SignedByDriverName,
                    deliveryDocument.WarehouseCode,
                    deliveryDocument.PartnersOrderNumber,
                    deliveryDocument.OperationalUnitCode,
                    deliveryDocument.OperationalUnitName,
                    deliveryDocument.WeightCalculated,
                    deliveryDocument.AdrPoints,
                    deliveryDocument.RznoCode,
                    deliveryDocument.CombinedRznoCode,
                    deliveryDocument.SigningDeviceCode,
                    0,
                    currentEvent.UploadedAt,
                    currentEvent.UploadedByUserName,
                    currentEvent.UploadedByUserFullName,
                    FinishMethod.ManuallyUploaded,
                    null));
        });
        
        ProjectEventAsync<LoadingDocumentSignedEvent>(async (session, docView, currentEvent) =>
        {
            var loadingDocument = await session.LoadAsync<LoadingDocument>(currentEvent.Id);

            if (loadingDocument is null)
                return;
            
            MapLoadingDocumentView(
                docView, loadingDocument, currentEvent.IsLoadingDocumentAlsoSigned, currentEvent.SignedByUserName, currentEvent.SignedByFullName, 
                currentEvent.SignedAt, FinishMethod.Signed, loadingDocument.PrintCopiesCount, currentEvent.DriverName, currentEvent.LicensePlate, docView.DeletionReason);
            
            foreach (var deliveryDocumentCode in currentEvent.DeliveryDocumentCodes)
            {
                var deliveryDocument = loadingDocument.DeliveryDocuments.FirstOrDefault(x => x.Code == deliveryDocumentCode);
                if (deliveryDocument is null)
                    continue;
                
                docView.DeliveryDocuments.Add(
                    new SignedDeliveryDocument(
                        deliveryDocument.Code,
                        deliveryDocument.LoadingDocumentCode,
                        deliveryDocument.PartnerCode,
                        deliveryDocument.PartnerNameShort,
                        deliveryDocument.DeliveryMethodCode,
                        deliveryDocument.DeliveryMethodName,
                        deliveryDocument.SignedWithLicensePlate,
                        deliveryDocument.SignedByDriverName,
                        deliveryDocument.WarehouseCode,
                        deliveryDocument.PartnersOrderNumber,
                        deliveryDocument.OperationalUnitCode,
                        deliveryDocument.OperationalUnitName,
                        deliveryDocument.WeightCalculated,
                        deliveryDocument.AdrPoints,
                        deliveryDocument.RznoCode,
                        deliveryDocument.CombinedRznoCode,
                        deliveryDocument.SigningDeviceCode,
                        deliveryDocument.PrintCopiesCount,
                        currentEvent.SignedAt,
                        currentEvent.SignedByUserName,
                        currentEvent.SignedByFullName,
                        FinishMethod.Signed,
                        null));
            }
        });
        
        ProjectEventAsync<LoadingDocumentDeletedEvent>(async (session, docView, currentEvent) =>
        {
            var loadingDocument = await session.LoadAsync<LoadingDocument>(currentEvent.Id);

            if (loadingDocument is null)
                return;
            
            MapLoadingDocumentView(
                docView, loadingDocument, currentEvent.IsLoadingDocumentAlsoDeleted, currentEvent.DeletedByUserName, currentEvent.DeletedByUserFullName, 
                currentEvent.DeletedAt, FinishMethod.Deleted, 0, docView.DriverName, docView.LicensePlate, currentEvent.DeletionReason);
            
            foreach (var deliveryDocumentCode in currentEvent.DeliveryDocumentCodes)
            {
                var deliveryDocument = loadingDocument.DeliveryDocuments.FirstOrDefault(x => x.Code == deliveryDocumentCode);
                if (deliveryDocument is null)
                    continue;
                
                docView.DeliveryDocuments.Add(
                    new SignedDeliveryDocument(
                        deliveryDocument.Code,
                        deliveryDocument.LoadingDocumentCode,
                        deliveryDocument.PartnerCode,
                        deliveryDocument.PartnerNameShort,
                        deliveryDocument.DeliveryMethodCode,
                        deliveryDocument.DeliveryMethodName,
                        deliveryDocument.SignedWithLicensePlate,
                        deliveryDocument.SignedByDriverName,
                        deliveryDocument.WarehouseCode,
                        deliveryDocument.PartnersOrderNumber,
                        deliveryDocument.OperationalUnitCode,
                        deliveryDocument.OperationalUnitName,
                        deliveryDocument.WeightCalculated,
                        deliveryDocument.AdrPoints,
                        deliveryDocument.RznoCode,
                        deliveryDocument.CombinedRznoCode,
                        deliveryDocument.SigningDeviceCode,
                        0,
                        currentEvent.DeletedAt,
                        currentEvent.DeletedByUserName,
                        currentEvent.DeletedByUserFullName,
                        FinishMethod.Deleted,
                        currentEvent.DeletionReason));
            }
        });

        ProjectEvent<LoadingDocumentWithAllDeliveryDocumentsFinishedEvent>((docView, currentEvent) =>
        {
            docView.IsDocumentFullyFinished = true;
        });
    }

    private static void MapLoadingDocumentView(
        SignedLoadingDocumentView docView,
        LoadingDocument loadingDocument,
        bool isLoadingDocumentAffected,
        string finishedByUserName,
        string? finishedByUserFullName,
        DateTimeOffset finishedAt,
        FinishMethod finishMethodType,
        int printedCopiesCount,
        string? signedByDriverName,
        string? signedWithLicensePlate,
        string? deletionReason)
    {
        docView.Code = loadingDocument.Code;
        docView.GateNumber = loadingDocument.GateNumber;
        docView.DepositorCode = loadingDocument.DepositorCode;
        docView.DeliveryMethodCode = loadingDocument.DeliveryMethodCode;
        docView.DeliveryMethodName = loadingDocument.DeliveryMethodName;
        docView.Weight = loadingDocument.Weight;
        docView.AdrPoints = loadingDocument.AdrPoints;
        docView.LoadingLocation = loadingDocument.LoadingLocation;
        docView.LoadingInWmsFinishedBy = loadingDocument.LoadingInWmsFinishedBy;
        docView.LoadingInWmsFinishedAt = loadingDocument.LoadingInWmsFinishedAt;
        docView.CreatedAt = loadingDocument.CreatedAt;
        docView.PrintedCopiesCount = loadingDocument.PrintCopiesCount;
        docView.DeliveryDocuments ??= [];
        
        if (isLoadingDocumentAffected)
        {
            docView.FinishedByUserName = finishedByUserName;
            docView.FinishedByUserFullName = finishedByUserFullName;
            docView.FinishedAt = finishedAt;
            docView.FinishMethodType = finishMethodType;
            docView.SigningDeviceCode = loadingDocument.SigningDeviceCode;
            docView.PrintedCopiesCount = printedCopiesCount;
            docView.DeletionReason = deletionReason;
            docView.LicensePlate = signedWithLicensePlate;
            docView.DriverName = signedByDriverName;
        }
    }
    
    private static Guid GenerateProjectionId(Guid documentId)
    {
        var newIdWithPrefix = $"SignedLoadingDocument-{documentId}";
        var bytes = System.Text.Encoding.UTF8.GetBytes(newIdWithPrefix);
        var hash = System.Security.Cryptography.MD5.HashData(bytes);
        return new Guid(hash);
    }
}