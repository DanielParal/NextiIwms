namespace Nexticz.Module.Sign.DocumentManager.Domain.LoadingDocumentAggregate;

internal record DocumentEmailMessage(
    LoadingDocument LoadingDocument, bool ShouldLoadingDocumentBeSent, string[] DeliveryDocumentCodesToSend);