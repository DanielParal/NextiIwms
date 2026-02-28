using Nexticz.Module.Sign.DocumentManager.Domain.SigningDeviceAggregate;

namespace Nexticz.Module.Sign.DocumentManager.Application.LoadingDocuments.Commands.RevertLoadingDocumentsFiles;

internal class RevertLoadingDocumentFileJob(
    string loadingDocumentCode,
    bool shouldLoadingDocumentBeReverted,
    string[] deliveryDocumentCodes)
{
    public string LoadingDocumentCode { get; private set; } = loadingDocumentCode;
    public bool ShouldLoadingDocumentBeReverted { get; private set; } = shouldLoadingDocumentBeReverted;
    public string[] DeliveryDocumentCodes { get; private set; } = deliveryDocumentCodes;

    public static RevertLoadingDocumentFileJob CreateFrom(SentLoadingDocument sentLoadingDocument)
    {
        return new RevertLoadingDocumentFileJob(
            sentLoadingDocument.LoadingDocumentCode,
            sentLoadingDocument.ShouldAlsoSendLoadingDocument,
            sentLoadingDocument.DeliveryDocuments.Select(x => x.Code).ToArray());
    }
}