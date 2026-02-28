using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Sign.DocumentManager.Domain.SigningDeviceAggregate.Events;

public record SigningDeviceDeletedEvent(Guid Id, string Code) : IMartenEvent;