using Nexticz.Module.Sign.DocumentManager.Contracts.LoadingDocuments;
using Nexticz.Module.Sign.DocumentManager.Contracts.SignedLoadingDocuments;
using Nexticz.Module.Sign.DocumentManager.Domain.Views;

namespace Nexticz.Module.Sign.DocumentManager.Presentation.SignedLoadingDocuments;

internal static class SignedLoadingDocumentResponseFactory
{
    public static SignedLoadingDocumentResponse Create(SignedLoadingDocumentView signedLoadingDocumentView)
    {
        return new SignedLoadingDocumentResponse(
            signedLoadingDocumentView.Code,
            signedLoadingDocumentView.GateNumber,
            signedLoadingDocumentView.DepositorCode,
            signedLoadingDocumentView.DeliveryMethodCode,
            signedLoadingDocumentView.DeliveryMethodName,
            signedLoadingDocumentView.LicensePlate,
            signedLoadingDocumentView.DriverName,
            signedLoadingDocumentView.Weight,
            signedLoadingDocumentView.AdrPoints,
            signedLoadingDocumentView.LoadingLocation,
            signedLoadingDocumentView.LoadingInWmsFinishedBy,
            signedLoadingDocumentView.LoadingInWmsFinishedAt,
            signedLoadingDocumentView.CreatedAt,
            signedLoadingDocumentView.FinishedAt,
            signedLoadingDocumentView.FinishedByUserName,
            signedLoadingDocumentView.FinishedByUserFullName,
            signedLoadingDocumentView.FinishMethodType is null ? null : (FinishMethodContract)signedLoadingDocumentView.FinishMethodType!,
            signedLoadingDocumentView.IsDocumentFullyFinished,
            signedLoadingDocumentView.DeletionReason,
            signedLoadingDocumentView.DeliveryDocuments
                .Select(dn => new SingedDeliveryDocumentContract(
                    dn.Code,
                    dn.LoadingDocumentCode,
                    dn.PartnerCode,
                    dn.PartnerNameShort,
                    dn.DeliveryMethodCode,
                    dn.DeliveryMethodName,
                    dn.LicensePlate,
                    dn.DriverName,
                    dn.WarehouseCode,
                    dn.PartnersOrderNumber,
                    dn.OperationalUnitCode,
                    dn.OperationalUnitName,
                    dn.WeightCalculated,
                    dn.AdrPoints,
                    dn.RznoCode,
                    dn.CombinedRznoCode,
                    dn.FinishedAt,
                    dn.FinishedByUserName,
                    dn.FinishedByUserFullName,
                    (FinishMethodContract)dn.FinishMethodType,
                    dn.DeletionReason
                ))
                .ToArray());
    }
}