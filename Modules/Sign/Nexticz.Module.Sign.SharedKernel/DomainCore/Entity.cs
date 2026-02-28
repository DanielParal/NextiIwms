namespace Nexticz.Module.Sign.SharedKernel.DomainCore;

public abstract class Entity : Lib.Shared.DomainCore.Entity
{
    protected Entity(Guid id) => Id = id;

    protected Entity() { }
}