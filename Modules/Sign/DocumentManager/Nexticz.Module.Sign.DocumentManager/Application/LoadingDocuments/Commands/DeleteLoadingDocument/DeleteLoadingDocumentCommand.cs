using ErrorOr;

namespace Nexticz.Module.Sign.DocumentManager.Application.LoadingDocuments.Commands.DeleteLoadingDocument;

internal record DeleteLoadingDocumentCommand(
    Guid LoadingDocumentId, string LoadingDocumentCode, bool ShouldLoadingDocumentBeAlsoDeleted, 
    string[] DeliveryDocumentCodes, string DeleteReason, DateTimeOffset DeletedAt, string DeletedByUserName, 
    string? DeletedByUserFullName) 
    : IDocumentManagerCommand<ErrorOr<Success>>;