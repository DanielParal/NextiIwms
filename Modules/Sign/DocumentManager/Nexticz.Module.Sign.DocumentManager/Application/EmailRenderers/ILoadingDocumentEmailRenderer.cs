using ErrorOr;
using Nexticz.Module.Sign.DocumentManager.Domain.LoadingDocumentAggregate;

namespace Nexticz.Module.Sign.DocumentManager.Application.EmailRenderers;

internal interface ILoadingDocumentEmailRenderer
{
    Task<ErrorOr<RenderedEmailResult>> RenderEmailAsync(LoadingDocument loadingDocument, CancellationToken cancellationToken);
}