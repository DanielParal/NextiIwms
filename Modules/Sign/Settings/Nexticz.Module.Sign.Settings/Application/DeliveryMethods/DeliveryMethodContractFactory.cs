using Nexticz.Module.Sign.Settings.Contracts.DeliveryMethods;
using Nexticz.Module.Sign.Settings.Domain.DeliveryMethodAggregate;

namespace Nexticz.Module.Sign.Settings.Application.DeliveryMethods;

internal static class DeliveryMethodContractFactory
{
    public static DeliveryMethodContract Create(DeliveryMethod deliveryMethod)
    {
        return new DeliveryMethodContract(
            deliveryMethod.Code, deliveryMethod.Name, 
            deliveryMethod.LoadingDocumentPrintCopiesCount, deliveryMethod.DeliveryDocumentPrintCopiesCount);
    }
}