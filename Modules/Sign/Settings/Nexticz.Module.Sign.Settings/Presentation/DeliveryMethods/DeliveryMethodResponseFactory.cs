using Nexticz.Module.Sign.Settings.Contracts.DeliveryMethods;
using Nexticz.Module.Sign.Settings.Domain.DeliveryMethodAggregate;

namespace Nexticz.Module.Sign.Settings.Presentation.DeliveryMethods;

internal static class DeliveryMethodResponseFactory
{
    public static DeliveryMethodResponse Create(DeliveryMethod deliveryMethod)
    {
        return new DeliveryMethodResponse(
            deliveryMethod.Id, deliveryMethod.Code, deliveryMethod.Name, deliveryMethod.LoadingDocumentPrintCopiesCount, deliveryMethod.DeliveryDocumentPrintCopiesCount);
    }
}