using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Mmo.Settings.Domain.ConstantEntity.Events;

public record ConstantCreatedEvent(
    Guid Id, string Key, string Value, ConstantType Type, string? Description) : IMartenEvent;