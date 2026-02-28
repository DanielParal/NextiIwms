namespace Nexticz.Module.Portal.Domain;

public class AggregateRoot : Lib.Shared.DomainCore.AggregateRoot
{
    protected AggregateRoot(Guid id) : base(id)
    {
    }

    protected AggregateRoot() { }
}