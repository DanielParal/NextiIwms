using ErrorOr;
using Nexticz.Module.Sign.DocumentLoader.Contracts;
using Nexticz.Module.Sign.DocumentManager.Domain.LoadingDocumentAggregate;

namespace Nexticz.Module.Sign.DocumentManager.Application.LoadingDocuments.Commands.CreateLoadingDocument;

internal record CreateLoadingDocumentCommand(
    LoadingDocumentContract LoadingDocumentContractFromLoader) 
    : IDocumentManagerCommand<ErrorOr<LoadingDocument>>;