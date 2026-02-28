using Nexticz.Module.Sign.SharedKernel.DomainCore;

namespace Nexticz.Module.Sign.Settings.Domain.DocumentTemplateAggregate;

public class TextBackground : ValueObject
{
    public string Name { get; set; }
    public double XPositionOffset { get; set; }
    public double Width { get; set; }

    public TextBackground(string name, double xPositionOffset, double width)
    {
        Name = name;
        XPositionOffset = xPositionOffset;
        Width = width;   
    }
    
    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Name;
        yield return XPositionOffset;
        yield return Width;
    }
}