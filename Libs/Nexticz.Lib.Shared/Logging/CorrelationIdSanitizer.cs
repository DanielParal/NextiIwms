using System.Text.RegularExpressions;
using Nexticz.Lib.Shared.Extensions;

namespace Nexticz.Lib.Shared.Logging;

internal sealed class CorrelationIdSanitizer
{
    private readonly int _maxLength;
    private readonly string[] _forbiddenWords =
    [
        "error",
        "warn",
        "fatal",
        "exception"
    ];

    private readonly Regex _nonWordChars = new(@"\W-", RegexOptions.Compiled);
    private readonly Regex _forbiddenWordsRegex;

    public CorrelationIdSanitizer(int maxCorrelationIdLength)
    {
        _maxLength = maxCorrelationIdLength;
        _forbiddenWordsRegex = new Regex(string.Join("|", _forbiddenWords), RegexOptions.Compiled | RegexOptions.IgnoreCase);
    }

    public string Sanitize(string value)
    {
        return string.IsNullOrEmpty(value)
            ? string.Empty
            : _forbiddenWordsRegex
                .Replace(_nonWordChars.Replace(value, string.Empty), string.Empty)
                .Truncate(_maxLength);
    }
}