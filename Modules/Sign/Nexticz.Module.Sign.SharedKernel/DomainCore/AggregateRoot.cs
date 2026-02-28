namespace Nexticz.Module.Sign.SharedKernel.DomainCore;

public class AggregateRoot : Lib.Shared.DomainCore.AggregateRoot
{
    protected AggregateRoot(Guid id) : base(id)
    {
    }

    protected AggregateRoot() { }
}