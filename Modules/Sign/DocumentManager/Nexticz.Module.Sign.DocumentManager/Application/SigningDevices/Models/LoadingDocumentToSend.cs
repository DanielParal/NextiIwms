namespace Nexticz.Module.Sign.DocumentManager.Application.SigningDevices.Models;

internal record LoadingDocumentToSend(
    string LoadingDocumentCode, 
    string[] DeliveryDocumentCodes,
    bool ShouldAlsoSendLoadingDocument);
    