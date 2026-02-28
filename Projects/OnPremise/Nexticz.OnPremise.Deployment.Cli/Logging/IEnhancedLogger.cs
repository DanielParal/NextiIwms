namespace Nexticz.OnPremise.Deployment.Cli.Logging;

internal interface IEnhancedLogger<T>
{
    void LogInformation(string message, bool logToConsole = false, bool highlightMessage = false);
    void LogWarning(string message, bool logToConsole = false);
    void LogError(string message, bool logToConsole = false);
    void LogCritical(string message, Exception exception);
}