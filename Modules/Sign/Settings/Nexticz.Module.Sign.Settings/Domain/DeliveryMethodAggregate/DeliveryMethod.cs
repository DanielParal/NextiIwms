using Nexticz.Module.Sign.Settings.Domain.DeliveryMethodAggregate.Events;
using Nexticz.Module.Sign.SharedKernel;
using Nexticz.Module.Sign.SharedKernel.DomainCore;

namespace Nexticz.Module.Sign.Settings.Domain.DeliveryMethodAggregate;

public class DeliveryMethod : AggregateRoot
{
    public string Code { get; private set; }
    public string Name { get; private set; }
    public int LoadingDocumentPrintCopiesCount { get; private set; }
    public int DeliveryDocumentPrintCopiesCount { get; private set; }
    
    // We need private constructor due to Marten deserialization
    private DeliveryMethod() {}
    
    public DeliveryMethod(
        string code, 
        string name, 
        int loadingDocumentPrintCopiesCount,
        int deliveryDocumentPrintCopiesCount,
        Guid? id = null) : base(id ?? Guid.NewGuid())
    {
        Code = code.ToUpperInvariant();
        Name = name;
        LoadingDocumentPrintCopiesCount = loadingDocumentPrintCopiesCount;
        DeliveryDocumentPrintCopiesCount = deliveryDocumentPrintCopiesCount;
    }

    public void Apply(DeliveryMethodCreatedEvent @event)
    {
        Code = @event.Code;
        Name = @event.Name;
        LoadingDocumentPrintCopiesCount = @event.LoadingDocumentPrintCopiesCount;
        DeliveryDocumentPrintCopiesCount = @event.DeliveryDocumentPrintCopiesCount;
    }
    
    public void Apply(DeliveryMethodUpdatedEvent @event)
    {
        Name = @event.Name;
        LoadingDocumentPrintCopiesCount = @event.LoadingDocumentPrintCopiesCount;
        DeliveryDocumentPrintCopiesCount = @event.DeliveryDocumentPrintCopiesCount;
    }
}