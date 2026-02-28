namespace Nexticz.Module.Mmo.SharedKernel.DomainCore;

public class AggregateRoot : Lib.Shared.DomainCore.AggregateRoot
{
    protected AggregateRoot(Guid id) : base(id)
    {
    }

    protected AggregateRoot() { }
}