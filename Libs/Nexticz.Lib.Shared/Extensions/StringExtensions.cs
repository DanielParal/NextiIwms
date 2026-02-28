namespace Nexticz.Lib.Shared.Extensions;

public static class StringExtension
{
    public static string ToCamelCase(this string str)
    {
        return string.IsNullOrEmpty(str) || str.Length < 2
            ? str.ToLowerInvariant()
            : char.ToLowerInvariant(str[0]) + str.Substring(1);
    }

    public static string ToFirstLetterToUpper(this string str)
    {
        return !string.IsNullOrWhiteSpace(str)
            ? string.Concat(str[0].ToString().ToUpper(), str.AsSpan(1))
            : "";
    }

    public static string Truncate(this string value, int maxLength)
    {
        return value.Length <= maxLength
            ? value
            : value[..maxLength];
    }
        
}