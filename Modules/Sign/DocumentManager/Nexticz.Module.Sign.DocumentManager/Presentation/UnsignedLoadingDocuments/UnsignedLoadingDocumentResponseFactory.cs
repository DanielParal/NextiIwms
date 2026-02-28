using Nexticz.Module.Sign.DocumentManager.Contracts.LoadingDocuments;
using Nexticz.Module.Sign.DocumentManager.Contracts.UnsignedLoadingDocuments;
using Nexticz.Module.Sign.DocumentManager.Domain.Views;

namespace Nexticz.Module.Sign.DocumentManager.Presentation.UnsignedLoadingDocuments;

internal static class UnsignedLoadingDocumentResponseFactory
{
    public static UnsignedLoadingDocumentResponse Create(UnsignedLoadingDocumentView unsignedLoadingDocumentView)
    {
        return new UnsignedLoadingDocumentResponse(
            unsignedLoadingDocumentView.Code,
            unsignedLoadingDocumentView.GateNumber,
            unsignedLoadingDocumentView.DepositorCode,
            unsignedLoadingDocumentView.DeliveryMethodCode,
            unsignedLoadingDocumentView.DeliveryMethodName,
            unsignedLoadingDocumentView.LicensePlate,
            unsignedLoadingDocumentView.DriverName,
            unsignedLoadingDocumentView.Weight,
            unsignedLoadingDocumentView.AdrPoints,
            unsignedLoadingDocumentView.LoadingLocation,
            unsignedLoadingDocumentView.LoadingInWmsFinishedBy,
            unsignedLoadingDocumentView.LoadingInWmsFinishedAt,
            unsignedLoadingDocumentView.CreatedAt,
            unsignedLoadingDocumentView.RequestedPrintCopiesCount,
            unsignedLoadingDocumentView.SigningDeviceCode,
            unsignedLoadingDocumentView.SentToSigningDeviceByUserName,
            unsignedLoadingDocumentView.SentToSigningDeviceAt,
            unsignedLoadingDocumentView.FinishedAt,
            unsignedLoadingDocumentView.FinishMethodType is null ? null : (FinishMethodContract)unsignedLoadingDocumentView.FinishMethodType!,
            unsignedLoadingDocumentView.DeletionReason,
            unsignedLoadingDocumentView.DeliveryDocuments
                .Select(dn => new UnsingedDeliveryDocumentContract(
                    dn.Code,
                    dn.LoadingDocumentCode,
                    dn.PartnerCode,
                    dn.PartnerNameShort,
                    dn.DeliveryMethodCode,
                    dn.DeliveryMethodName,
                    dn.WarehouseCode,
                    dn.PartnersOrderNumber,
                    dn.OperationalUnitCode,
                    dn.OperationalUnitName,
                    dn.RequestedPrintCopiesCount,
                    dn.WeightCalculated,
                    dn.AdrPoints,
                    dn.RznoCode,
                    dn.CombinedRznoCode,
                    dn.SigningDeviceCode,
                    dn.SentToSigningDeviceByUserName,
                    dn.SentToSigningDeviceAt,
                    dn.FinishedAt,
                    dn.FinishMethodType is null ? null : (FinishMethodContract)dn.FinishMethodType!,
                    dn.DeletionReason
                ))
                .ToArray());
    }
}