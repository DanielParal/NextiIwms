namespace Nexticz.Lib.Shared.DomainCore;

public abstract class AggregateRoot : Entity
{
    protected AggregateRoot(Guid id) : base(id)
    {
    }

    protected AggregateRoot() { }
    
    protected static Guid GenerateIdFromString(string lineCode)
    {
        var lineBytes = System.Text.Encoding.UTF8.GetBytes(lineCode);
        var hash = System.Security.Cryptography.MD5.HashData(lineBytes);
        return new Guid(hash);
    }
}