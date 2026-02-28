using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Sign.Settings.Domain.ConstantAggregate.Events;

public record ConstantDeletedEvent(Guid Id, string Key) : IMartenEvent;