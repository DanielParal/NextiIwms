using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Mmo.Settings.Domain.UserEntity.Events;

public record UserUpdatedFromAuthModuleEvent(Guid Id, bool IsActive, string[] Roles, string[] Permissions) : IMartenEvent;