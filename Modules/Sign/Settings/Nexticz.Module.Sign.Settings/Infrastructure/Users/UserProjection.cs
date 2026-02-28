using JasperFx.Events;
using Marten.Events.Aggregation;
using Nexticz.Module.Sign.Settings.Domain.UserAggregate;
using Nexticz.Module.Sign.Settings.Domain.UserAggregate.Events;

namespace Nexticz.Module.Sign.Settings.Infrastructure.Users;

public class UserProjection : SingleStreamProjection<User, Guid>
{
    public UserProjection()
    {
        DeleteEvent<UserDeletedFromAuthModuleEvent>();
    }
    
    public void Apply(IEvent<UserCreatedFromAuthModuleEvent> @event, User user)
    {
        user.Apply(@event.Data);
    }
    
    public void Apply(IEvent<UserUpdatedFromAuthModuleEvent> @event, User user)
    {
        user.Apply(@event.Data);
    }
    
    public void Apply(IEvent<UserUpdatedEvent> @event, User user)
    {
        user.Apply(@event.Data);
    }
    
    public void Apply(IEvent<UserSignatureUploadedEvent> @event, User user)
    {
        user.Apply(@event.Data);
    }
    
    public void Apply(IEvent<UserSignatureDeletedEvent> @event, User user)
    {
        user.Apply(@event.Data);
    }
}