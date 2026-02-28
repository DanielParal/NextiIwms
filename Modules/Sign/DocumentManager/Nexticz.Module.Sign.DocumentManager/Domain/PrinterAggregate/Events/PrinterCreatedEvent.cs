using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Sign.DocumentManager.Domain.PrinterAggregate.Events;

public record PrinterCreatedEvent(Guid Id, string Code, string Ip) : IMartenEvent;