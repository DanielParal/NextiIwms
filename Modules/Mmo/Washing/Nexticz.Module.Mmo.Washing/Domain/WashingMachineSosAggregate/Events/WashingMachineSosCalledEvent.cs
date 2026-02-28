using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Mmo.Washing.Domain.WashingMachineSosAggregate.Events;

public record WashingMachineSosCalledEvent(Guid Id, string Code, DateTimeOffset SosCalledAt) : IMartenEvent;