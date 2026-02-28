namespace Nexticz.Module.Sign.Settings.Domain.ConstantAggregate;

internal class ConstantValidator
{
    public static bool IsValidValue(string value, ConstantType type)
    {
        return type switch
        {
            ConstantType.String => true,
            ConstantType.Int32 => int.TryParse(value, out _),
            ConstantType.Boolean => bool.TryParse(value, out _),
            ConstantType.Double => double.TryParse(value, out _),
            ConstantType.DateTimeOffset => DateTimeOffset.TryParse(value, out _),
            ConstantType.Decimal => decimal.TryParse(value, out _),
            _ => false
        };
    }
}