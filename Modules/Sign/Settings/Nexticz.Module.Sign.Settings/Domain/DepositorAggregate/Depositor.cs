using Nexticz.Module.Sign.Settings.Domain.DepositorAggregate.Events;
using Nexticz.Module.Sign.SharedKernel.DomainCore;

namespace Nexticz.Module.Sign.Settings.Domain.DepositorAggregate;

public class Depositor : AggregateRoot
{
    public string Code { get; private set; }
    public string Name { get; private set; }
    public string DepositorGroupCode { get; private set; }
    public string DeliveryTemplateCode { get; private set; }
    public string LoadingTemplateCode { get; private set; }
    
    // We need private constructor due to Marten deserialization
    private Depositor() {}
    
    public Depositor(
        string code, 
        string name, 
        string depositorGroupCode,
        string deliveryTemplateCode,
        string loadingTemplateCode,
        Guid? id = null) : base(id ?? Guid.NewGuid())
    {
        Code = code.ToUpperInvariant();
        Name = name;
        DepositorGroupCode = depositorGroupCode;
        DeliveryTemplateCode = deliveryTemplateCode;
        LoadingTemplateCode = loadingTemplateCode;
    }

    public void Apply(DepositorCreatedEvent @event)
    {
        Code = @event.Code;
        Name = @event.Name;
        DepositorGroupCode = @event.DepositorGroupCode;
        DeliveryTemplateCode = @event.DeliveryTemplateCode;
        LoadingTemplateCode = @event.LoadingTemplateCode;       
    }
    
    public void Apply(DepositorUpdatedEvent @event)
    {
        Name = @event.Name;
        DepositorGroupCode = @event.DepositorGroupCode;
        DeliveryTemplateCode = @event.DeliveryTemplateCode;
        LoadingTemplateCode = @event.LoadingTemplateCode;       
    }
}