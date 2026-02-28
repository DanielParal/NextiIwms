using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Mmo.Settings.Domain.ConstantEntity.Events;

public record ConstantDeletedEvent(Guid Id, string Key) : IMartenEvent;