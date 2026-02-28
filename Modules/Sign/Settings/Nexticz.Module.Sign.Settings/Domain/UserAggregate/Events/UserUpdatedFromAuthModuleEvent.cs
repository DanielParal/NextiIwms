using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Sign.Settings.Domain.UserAggregate.Events;

public record UserUpdatedFromAuthModuleEvent(Guid Id, string UserName, string? FullName, bool IsActive, string[] Roles, string[] Permissions) : IMartenEvent;