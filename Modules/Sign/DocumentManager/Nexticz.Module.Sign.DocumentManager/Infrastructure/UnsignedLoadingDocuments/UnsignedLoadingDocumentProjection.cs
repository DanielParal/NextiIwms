using Marten.Events.Projections;
using Nexticz.Module.Sign.DocumentManager.Domain.LoadingDocumentAggregate;
using Nexticz.Module.Sign.DocumentManager.Domain.LoadingDocumentAggregate.Events;
using Nexticz.Module.Sign.DocumentManager.Domain.Views;

namespace Nexticz.Module.Sign.DocumentManager.Infrastructure.UnsignedLoadingDocuments;

public class UnsignedLoadingDocumentProjection : MultiStreamProjection<UnsignedLoadingDocumentView, Guid>
{
    public UnsignedLoadingDocumentProjection()
    {
        Identity<LoadingDocumentCreatedEvent>(x => GenerateProjectionId(x.Id));
        Identity<LoadingDocumentToSigningDeviceSentEvent>(x => GenerateProjectionId(x.Id));
        Identity<LoadingDocumentFromSigningDeviceReturnedEvent>(x => GenerateProjectionId(x.Id));
        Identity<LoadingDocumentSignedEvent>(x => GenerateProjectionId(x.Id));
        Identity<SignedDocumentManuallyUploadedEvent>(x => GenerateProjectionId(x.Id));
        Identity<LoadingDocumentPrintCopiesCountChangedEvent>(x => GenerateProjectionId(x.Id));
        Identity<LoadingDocumentWithAllDeliveryDocumentsFinishedEvent>(x => GenerateProjectionId(x.Id));
        Identity<LoadingDocumentDeletedEvent>(x => GenerateProjectionId(x.Id));
        
        DeleteEvent<LoadingDocumentWithAllDeliveryDocumentsFinishedEvent>();
        
        ProjectEvent<LoadingDocumentCreatedEvent>((docView, currentEvent) =>
        {
            docView.Code = currentEvent.Code;
            docView.GateNumber = currentEvent.GateNumber;
            docView.DepositorCode = currentEvent.DepositorCode;
            docView.DeliveryMethodCode = currentEvent.DeliveryMethodCode;
            docView.DeliveryMethodName = currentEvent.DeliveryMethodName;
            docView.LicensePlate = currentEvent.LicensePlate;
            docView.DriverName = currentEvent.DriverName;
            docView.Weight = currentEvent.Weight;
            docView.AdrPoints = currentEvent.AdrPoints;
            docView.LoadingLocation = currentEvent.LoadingLocation;
            docView.LoadingInWmsFinishedBy = currentEvent.LoadingInWmsFinishedBy;
            docView.LoadingInWmsFinishedAt = currentEvent.LoadingInWmsFinishedAt;
            docView.CreatedAt = currentEvent.CreatedAt;
            docView.RequestedPrintCopiesCount = currentEvent.RequestedPrintCopiesCount;
            docView.DeletionReason = null;
            
            docView.DeliveryDocuments = currentEvent.DeliveryDocuments.Select(
                d => new UnsignedDeliveryDocument(
                    d.Code,
                    d.LoadingDocumentCode,
                    d.PartnerCode,
                    d.PartnerNameShort,
                    d.DeliveryMethodCode,
                    d.DeliveryMethodName,
                    d.WarehouseCode,
                    d.PartnersOrderNumber,
                    d.OperationalUnitCode,
                    d.OperationalUnitName,
                    d.WeightCalculated,
                    d.AdrPoints,
                    d.RznoCode,
                    d.CombinedRznoCode,
                    d.PrintCopiesCount,
                    d.SigningDeviceCode,
                    d.SentToSigningDeviceAt,
                    d.SentToSigningDeviceByUserName,
                    d.FinishedAt,
                    d.FinishMethodType,
                    null))
                .ToArray();
        });
        
        ProjectEvent<LoadingDocumentToSigningDeviceSentEvent>((docView, currentEvent) => 
        { 
            if (currentEvent.IsLoadingDocumentAlsoSent)
            {
                docView.SigningDeviceCode = currentEvent.SigningDeviceCode;
                docView.SentToSigningDeviceAt = currentEvent.SentAt;
                docView.SentToSigningDeviceByUserName = currentEvent.SentByUserName;
            }
        
            var deliveryDocuments = docView.DeliveryDocuments.Where(x => currentEvent.DeliveryDocumentCodes.Contains(x.Code)).ToArray();
        
            foreach (var deliveryDocument in deliveryDocuments)
            {
                deliveryDocument.SigningDeviceCode = currentEvent.SigningDeviceCode;
                deliveryDocument.SentToSigningDeviceAt = currentEvent.SentAt;
                deliveryDocument.SentToSigningDeviceByUserName = currentEvent.SentByUserName;
            }
        });
        
        ProjectEvent<LoadingDocumentFromSigningDeviceReturnedEvent>((docView, currentEvent) => 
        { 
            if (currentEvent.IsLoadingDocumentAlsoReturned)
            {
                docView.SigningDeviceCode = null;
                docView.SentToSigningDeviceAt = null;
                docView.SentToSigningDeviceByUserName = null;
            }
        
            var deliveryDocuments = docView.DeliveryDocuments.Where(x => currentEvent.DeliveryDocumentCodes.Contains(x.Code)).ToArray();
        
            foreach (var deliveryDocument in deliveryDocuments)
            {
                deliveryDocument.SigningDeviceCode = null;
                deliveryDocument.SentToSigningDeviceAt = null;
                deliveryDocument.SentToSigningDeviceByUserName = null;
            }
        });

        ProjectEvent<SignedDocumentManuallyUploadedEvent>((docView, currentEvent) =>
        {
            if (currentEvent.IsLoadingDocumentSignature)
            {
                docView.FinishedAt = currentEvent.UploadedAt;
                docView.FinishMethodType = FinishMethod.ManuallyUploaded;
                return;
            }
            
            var deliveryDocument = docView.DeliveryDocuments.First(x => x.Code == currentEvent.DeliveryDocumentCode);
            deliveryDocument.FinishedAt = currentEvent.UploadedAt;
            deliveryDocument.FinishMethodType = FinishMethod.ManuallyUploaded;
        });
        
        ProjectEvent<LoadingDocumentSignedEvent>((docView, currentEvent) =>
        {
            if (currentEvent.IsLoadingDocumentAlsoSigned)
            {
                docView.SentToSigningDeviceAt = null;
                docView.SentToSigningDeviceByUserName = null;
                docView.FinishedAt = currentEvent.SignedAt;
                docView.FinishMethodType = FinishMethod.Signed;
            }
            
            foreach (var deliveryDocumentCode in currentEvent.DeliveryDocumentCodes)
            {
                var deliveryDocument = docView.DeliveryDocuments.FirstOrDefault(x => x.Code == deliveryDocumentCode);
                
                if (deliveryDocument is null)
                    continue;
                
                deliveryDocument.SentToSigningDeviceAt = null;
                deliveryDocument.SentToSigningDeviceByUserName = null;
                deliveryDocument.FinishedAt = currentEvent.SignedAt;
                deliveryDocument.FinishMethodType = FinishMethod.Signed;
            }
        });
        
        ProjectEvent<LoadingDocumentDeletedEvent>((docView, currentEvent) =>
        {
            if (currentEvent.IsLoadingDocumentAlsoDeleted)
            {
                docView.SentToSigningDeviceAt = null;
                docView.SentToSigningDeviceByUserName = null;
                docView.FinishedAt = currentEvent.DeletedAt;
                docView.FinishMethodType = FinishMethod.Deleted;
                docView.DeletionReason = currentEvent.DeletionReason;
            }
            
            foreach (var deliveryDocumentCode in currentEvent.DeliveryDocumentCodes)
            {
                var deliveryDocument = docView.DeliveryDocuments.FirstOrDefault(x => x.Code == deliveryDocumentCode);
                
                if (deliveryDocument is null)
                    continue;
                
                deliveryDocument.SentToSigningDeviceAt = null;
                deliveryDocument.SentToSigningDeviceByUserName = null;
                deliveryDocument.FinishedAt = currentEvent.DeletedAt;
                deliveryDocument.FinishMethodType = FinishMethod.Deleted;
                deliveryDocument.DeletionReason = currentEvent.DeletionReason;
            }
        });
        
        ProjectEvent<LoadingDocumentPrintCopiesCountChangedEvent>((docView, currentEvent) =>
        {
            if (string.IsNullOrWhiteSpace(currentEvent.DeliveryDocumentCode))
            {
                docView.RequestedPrintCopiesCount = currentEvent.PrintCopiesCount;
                return;
            }
            
            var deliveryDocument = docView.DeliveryDocuments.FirstOrDefault(x => x.Code == currentEvent.DeliveryDocumentCode);
                
            if (deliveryDocument is null)
                return;
            
            deliveryDocument.RequestedPrintCopiesCount = currentEvent.PrintCopiesCount;
        });
    }
    
    private static Guid GenerateProjectionId(Guid documentId)
    {
        var newIdWithPrefix = $"UnsignedLoadingDocument-{documentId}";
        var bytes = System.Text.Encoding.UTF8.GetBytes(newIdWithPrefix);
        var hash = System.Security.Cryptography.MD5.HashData(bytes);
        return new Guid(hash);
    }
}