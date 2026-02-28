using Nexticz.Module.Mmo.SharedKernel;
using Nexticz.Module.Mmo.SharedKernel.DomainCore;

namespace Nexticz.Module.Mmo.Settings.Domain.PackagingAggregate;

/// <summary>
/// Represents the dimensions of a package in millimeters (mm).
/// </summary>
public sealed class Dimensions : ValueObject
{
    public decimal Depth { get; }
    public decimal Width { get; }
    public decimal Height { get; }
    
    public Dimensions(decimal depth, decimal width, decimal height)
    {
        if (depth <= 0)
            throw new ArgumentException("Depth must be greater than 0.", nameof(depth));
        if (width <= 0)
            throw new ArgumentException("Width must be greater than 0.", nameof(width));
        if (height <= 0)
            throw new ArgumentException("Height must be greater than 0.", nameof(height));

        Depth = depth;
        Width = width;
        Height = height;
    }
    
    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Depth;
        yield return Width;
        yield return Height;
    }
}