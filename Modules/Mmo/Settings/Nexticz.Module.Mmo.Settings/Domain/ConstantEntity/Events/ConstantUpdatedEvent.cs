using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Mmo.Settings.Domain.ConstantEntity.Events;

public record ConstantUpdatedEvent(string Value, string? Description) : IMartenEvent;