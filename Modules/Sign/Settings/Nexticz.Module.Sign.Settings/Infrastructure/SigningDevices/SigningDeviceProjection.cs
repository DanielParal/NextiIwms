using JasperFx.Events;
using Marten.Events.Aggregation;
using Nexticz.Module.Sign.Settings.Domain.SigningDeviceAggregate;
using Nexticz.Module.Sign.Settings.Domain.SigningDeviceAggregate.Events;

namespace Nexticz.Module.Sign.Settings.Infrastructure.SigningDevices;

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
}