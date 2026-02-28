namespace Nexticz.Lib.Shared.DomainCore;

public abstract class Entity
{
    public Guid Id { get; protected set; }

    public override bool Equals(object? other)
    {
        if (other is null || other.GetType() != GetType())
        {
            return false;
        }

        return ((Entity)other).Id == Id;
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    protected Entity(Guid id) => Id = id;

    protected Entity() { }
}