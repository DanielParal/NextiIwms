using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;
using Nexticz.Module.Sign.SharedKernel;
using Nexticz.Module.Sign.SharedKernel.DomainCore;

namespace Nexticz.Module.Sign.Settings.Domain.DeliveryMethodAggregate.Events;

public class DeliveryMethodCreatedEvent(
    Guid id, string code, string name, 
    int loadingDocumentPrintCopiesCount, int deliveryDocumentPrintCopiesCount)
    : EventWithCode(code), IMartenEvent
{
    public Guid Id { get; } = id;
    public string Name { get; } = name;
    public int LoadingDocumentPrintCopiesCount { get; } = loadingDocumentPrintCopiesCount;
    public int DeliveryDocumentPrintCopiesCount { get; } = deliveryDocumentPrintCopiesCount;
}