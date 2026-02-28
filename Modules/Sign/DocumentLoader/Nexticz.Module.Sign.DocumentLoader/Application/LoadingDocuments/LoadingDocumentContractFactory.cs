using Nexticz.Module.Sign.DocumentLoader.Contracts;
using Nexticz.Module.Sign.DocumentLoader.Application.FileProcessors.Models;

namespace Nexticz.Module.Sign.DocumentLoader.Application.LoadingDocuments;

internal static class LoadingDocumentContractFactory
{
    public static LoadingDocumentContract Create(LoadingListFromXml loadingListFromXml)
    {
        var depositorCode = loadingListFromXml.DeliveryNotes.First().DepositorCode;
        return new LoadingDocumentContract(
            loadingListFromXml.Code,
            loadingListFromXml.GateNumber,
            depositorCode,
            loadingListFromXml.DeliveryMethodCode,
            loadingListFromXml.LicensePlate,
            loadingListFromXml.DriverName,
            loadingListFromXml.Weight,
            loadingListFromXml.AdrPoints,
            loadingListFromXml.LoadingLocation,
            loadingListFromXml.LoadingInWmsFinishedBy,
            loadingListFromXml.LoadingInWmsFinishedAt,
            loadingListFromXml.DeliveryNotes
                .Select(dn => 
                    new DeliveryDocumentContract(
                        dn.Code,
                        dn.LoadingListCode,
                        dn.DepositorCode,
                        dn.PartnerCode,
                        dn.PartnerNameShort,
                        dn.DeliveryMethodCode,
                        dn.WarehouseCode,
                        dn.PartnersOrderNumber,
                        dn.OperationalUnitCode,
                        dn.OperationalUnitName,
                        dn.IssueDate,
                        dn.RznoCode,
                        dn.CombinedRznoCode,
                        dn.WeightCalculated,
                        dn.AdrPoints))
                .ToArray());
    }
}