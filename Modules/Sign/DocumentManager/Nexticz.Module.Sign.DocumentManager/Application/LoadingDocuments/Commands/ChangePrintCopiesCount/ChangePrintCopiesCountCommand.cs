using ErrorOr;

namespace Nexticz.Module.Sign.DocumentManager.Application.LoadingDocuments.Commands.ChangePrintCopiesCount;

internal record ChangePrintCopiesCountCommand(string LoadingDocumentCode, string? DeliveryDocumentCode, int PrintCopiesCount) 
    : IDocumentManagerCommand<ErrorOr<Success>>;