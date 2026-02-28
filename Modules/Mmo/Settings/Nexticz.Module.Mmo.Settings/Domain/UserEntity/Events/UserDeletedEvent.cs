using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Mmo.Settings.Domain.UserEntity.Events;

public record UserDeletedEvent(Guid Id, string UserName) : IMartenEvent;