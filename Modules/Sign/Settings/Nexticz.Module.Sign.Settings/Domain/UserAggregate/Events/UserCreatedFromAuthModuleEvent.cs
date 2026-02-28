using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Sign.Settings.Domain.UserAggregate.Events;

public record UserCreatedFromAuthModuleEvent(Guid Id, string UserName, string? FullName, string[] Roles, string[] Permissions) : IMartenEvent;