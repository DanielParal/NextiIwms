using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Sign.Settings.Domain.UserAggregate.Events;

public record UserUpdatedEvent(
    Guid Id, 
    string UserName, 
    string[] DepositorCodes, 
    string[] DepositorGroupCodes,
    string[] SigningDeviceCodes) : IMartenEvent;