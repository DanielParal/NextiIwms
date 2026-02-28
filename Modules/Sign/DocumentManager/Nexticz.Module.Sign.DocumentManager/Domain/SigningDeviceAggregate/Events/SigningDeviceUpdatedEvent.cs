using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Sign.DocumentManager.Domain.SigningDeviceAggregate.Events;

public record SigningDeviceUpdatedEvent(Guid Id, string Code, string Name, bool IsActive, string PrinterCode) : IMartenEvent;