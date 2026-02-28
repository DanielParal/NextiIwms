using ClosedXML.Excel;

namespace Nexticz.Lib.Shared.Extensions;

public static class ClosedXmlExtensions
{
    public static bool TryGetIntValue(this IXLCell cell, out int value)
    {
        try
        {
            value = cell.GetValue<int>();
            return true;
        }
        catch
        {
            var stringValue = cell.GetValue<string>().Trim();
            return int.TryParse(stringValue, out value);
        }
    }
    
    public static bool TryGetDecimalValue(this IXLCell cell, out decimal value)
    {
        try
        {
            value = cell.GetValue<decimal>();
            return true;
        }
        catch
        {
            var stringValue = cell.GetValue<string>().Trim();
            return decimal.TryParse(stringValue, out value);
        }
    }
    
    public static string GetTrimString(this IXLCell cell)
    {
        var value = cell.GetValue<string>();
        
        return value is null ? string.Empty : value.Trim();
    }
}