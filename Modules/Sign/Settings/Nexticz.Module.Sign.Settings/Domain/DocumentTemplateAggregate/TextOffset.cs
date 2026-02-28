using Nexticz.Module.Sign.SharedKernel.DomainCore;

namespace Nexticz.Module.Sign.Settings.Domain.DocumentTemplateAggregate;

public class TextOffset : ValueObject
{
    public string Name { get; }
    public double Left { get; }
    public double Bottom { get; }
    public double Width { get; }
    public double Height { get; }

    public TextOffset(string name, double left, double bottom, double width, double height)
    {
        Name = name;
        Left = left;
        Bottom = bottom;
        Width = width;
        Height = height;   
    }
    
    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Name;
        yield return Left;
        yield return Bottom;
        yield return Width;
        yield return Height;
    }
}