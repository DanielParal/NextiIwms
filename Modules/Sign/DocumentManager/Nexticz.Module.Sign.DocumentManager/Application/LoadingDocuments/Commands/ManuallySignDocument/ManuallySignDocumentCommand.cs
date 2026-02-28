using ErrorOr;
using Microsoft.AspNetCore.Http;

namespace Nexticz.Module.Sign.DocumentManager.Application.LoadingDocuments.Commands.ManuallySignDocument;

internal record ManuallySignDocumentCommand(string LoadingDocumentCode, string? DeliveryDocumentCode, IFormFile FormFile): IDocumentManagerCommand<ErrorOr<Success>>;