using ErrorOr;
using Nexticz.Module.Sign.DocumentManager.Domain.LoadingDocumentAggregate;

namespace Nexticz.Module.Sign.DocumentManager.Application.EmailRenderers;

internal interface IDeliveryDocumentEmailRenderer
{
    Task<ErrorOr<RenderedEmailResult>> RenderEmailAsync(DeliveryDocument deliveryDocument, string? depositorName, DateTimeOffset loadingFinishedInWmsAt, CancellationToken cancellationToken);
}