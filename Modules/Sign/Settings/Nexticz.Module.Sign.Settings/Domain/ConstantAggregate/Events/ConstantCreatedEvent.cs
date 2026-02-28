using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Sign.Settings.Domain.ConstantAggregate.Events;

public record ConstantCreatedEvent(
    Guid Id, string Key, string Value, ConstantType Type, string? Description) : IMartenEvent;