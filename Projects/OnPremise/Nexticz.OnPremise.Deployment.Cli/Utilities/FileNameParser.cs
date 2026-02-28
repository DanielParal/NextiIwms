using ErrorOr;

namespace Nexticz.OnPremise.Deployment.Cli.Utilities;

internal static class FileNameParser
{
    public static ErrorOr<string> GetLastPart(string input)
    {
        var lastHyphenIndex = input.LastIndexOf('-');
        
        if (lastHyphenIndex == 0)
        {
            return Error.NotFound(ErrorMessages.FileNameParserNotFoundCode, ErrorMessages.FileNameParserNotFoundDescription(input));
        }
        
        var lastPart =  input[(lastHyphenIndex + 1)..];

        return string.IsNullOrEmpty(lastPart) ? Error.NotFound(ErrorMessages.FileNameParserNotFoundCode, ErrorMessages.FileNameParserNotFoundDescription(input)) : lastPart;
    }
}