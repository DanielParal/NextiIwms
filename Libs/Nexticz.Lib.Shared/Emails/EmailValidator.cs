using System.Text.RegularExpressions;

namespace Nexticz.Lib.Shared.Emails;

public static class EmailValidator
{
    // ^[^@\s]+: Starts with one or more characters that are not @ or whitespace (local part).
    //     @: Matches the @ symbol.
    //     [^@\s]+: Followed by one or more characters that are not @ or whitespace (domain).
    //     \.[^@\s]+$: Ends with a dot followed by one or more characters that are not @ or whitespace (top-level domain)
    private const string EmailRegexPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
    
    public static bool IsValidEmail(string email) => !string.IsNullOrWhiteSpace(email) && new Regex(EmailRegexPattern).IsMatch(email);
}