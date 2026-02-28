using JasperFx.Events;
using Marten.Events.Aggregation;
using Nexticz.Module.Mmo.Settings.Domain.UserEntity;
using Nexticz.Module.Mmo.Settings.Domain.UserEntity.Events;

namespace Nexticz.Module.Mmo.Settings.Infrastructure.Users;

public class UserProjection : SingleStreamProjection<User, Guid>
{
    public void Apply(IEvent<UserCreatedEvent> @event, User user)
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
    
    public User? Apply(IEvent<UserDeletedEvent> @event, User user)
    {
        return null;
    }
}