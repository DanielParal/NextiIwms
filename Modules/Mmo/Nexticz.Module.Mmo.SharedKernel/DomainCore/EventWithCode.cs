namespace Nexticz.Module.Mmo.SharedKernel.DomainCore;

public abstract class EventWithCode
{
    private string _code;

    protected EventWithCode(string code)
    {
        _code = code.ToUpperInvariant();
    }

    public string Code
    {
        get => _code;
        protected set => _code = value.ToUpperInvariant();
    }
}