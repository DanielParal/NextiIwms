using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Mmo.Washing.Domain.WashingMachineSosAggregate.Events;

public record WashingMachineSosResolvedEvent(Guid Id, string Code, DateTimeOffset SosResolvedAt) : IMartenEvent;