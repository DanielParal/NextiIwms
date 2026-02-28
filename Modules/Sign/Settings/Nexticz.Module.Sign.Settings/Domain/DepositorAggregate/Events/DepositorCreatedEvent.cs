using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;
using Nexticz.Module.Sign.SharedKernel.DomainCore;

namespace Nexticz.Module.Sign.Settings.Domain.DepositorAggregate.Events;

public class DepositorCreatedEvent(Guid id, string code, string name, string depositorGroupCode, string deliveryTemplateCode, string loadingTemplateCode)
    : EventWithCode(code), IMartenEvent
{
    public Guid Id { get; } = id;
    public string Name { get; } = name;
    public string DepositorGroupCode { get; } = depositorGroupCode;
    public string DeliveryTemplateCode { get; } = deliveryTemplateCode;
    public string LoadingTemplateCode { get; } = loadingTemplateCode;   
}