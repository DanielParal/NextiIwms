using JasperFx.Events;
using Marten.Events.Aggregation;
using Nexticz.Module.Sign.DocumentManager.Domain.SigningDeviceAggregate;
using Nexticz.Module.Sign.DocumentManager.Domain.SigningDeviceAggregate.Events;

namespace Nexticz.Module.Sign.DocumentManager.Infrastructure.SigningDevices;

public class SigningDeviceProjection : SingleStreamProjection<SigningDevice, Guid>
{
    public SigningDeviceProjection()
    {
        DeleteEvent<SigningDeviceDeletedEvent>();
    }
    
    public void Apply(IEvent<SigningDeviceCreatedEvent> @event, SigningDevice signingDevice)
    {
        signingDevice.Apply(@event.Data);
    }
    
    public void Apply(IEvent<SigningDeviceUpdatedEvent> @event, SigningDevice signingDevice)
    {
        signingDevice.Apply(@event.Data);
    }
    
    public void Apply(IEvent<DocumentsFromSigningDeviceReturnedEvent> @event, SigningDevice signingDevice)
    {
        signingDevice.Apply(@event.Data);
    }
    
    public void Apply(IEvent<DocumentsToSigningDeviceSentEvent> @event, SigningDevice signingDevice)
    {
        signingDevice.Apply(@event.Data);
    }
    
    public void Apply(IEvent<SigningResultSavedEvent> @event, SigningDevice signingDevice)
    {
        signingDevice.Apply(@event.Data);
    }
}