using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;
using Nexticz.Module.Mmo.SharedKernel;
using Nexticz.Module.Mmo.SharedKernel.DomainCore;

namespace Nexticz.Module.Mmo.Settings.Domain.PackagingCirculationEntity.Events;

public class PackagingCirculationCreatedEvent(Guid id, string code, string name)
    : EventWithCode(code), IMartenEvent
{
    public Guid Id { get; } = id;
    public string Name { get; } = name;
}