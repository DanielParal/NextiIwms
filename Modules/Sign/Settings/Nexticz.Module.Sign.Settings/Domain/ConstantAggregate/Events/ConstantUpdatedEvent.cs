using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Sign.Settings.Domain.ConstantAggregate.Events;

public record ConstantUpdatedEvent(string Value, string? Description) : IMartenEvent;