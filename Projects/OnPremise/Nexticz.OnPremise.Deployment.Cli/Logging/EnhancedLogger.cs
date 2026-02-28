using Microsoft.Extensions.Logging;

namespace Nexticz.OnPremise.Deployment.Cli.Logging;

internal class EnhancedLogger<T> : IEnhancedLogger<T>
{
    private readonly ILogger<T> _logger;
    private const string ClassNamePushProperty = "ClassName";

    public EnhancedLogger(ILogger<T> logger)
    {
        _logger = logger;
    }

    public void LogInformation(string message, bool logToConsole = false, bool highlightMessage = false)
    {
        using (Serilog.Context.LogContext.PushProperty(ClassNamePushProperty, typeof(T).Name))
        {
            _logger.LogInformation(message);
        }

        if (!logToConsole)
        {
            return;
        }

        if (highlightMessage)
        {
            Console.ForegroundColor = ConsoleColor.Green;
        }
        
        Console.WriteLine(message);
        Console.ResetColor();
    }
    
    public void LogWarning(string message, bool logToConsole = false)
    {
        using (Serilog.Context.LogContext.PushProperty(ClassNamePushProperty, typeof(T).Name))
        {
            _logger.LogWarning(message);
        }
        
        if (!logToConsole)
        {
            return;
        }
        
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    public void LogError(string message, bool logToConsole = false)
    {
        using (Serilog.Context.LogContext.PushProperty(ClassNamePushProperty, typeof(T).Name))
        {
            _logger.LogError(message);
        }
        
        if (!logToConsole)
        {
            return;
        }
        
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(message);
        Console.ResetColor();
    }
    
    public void LogCritical(string message, Exception exception)
    {
        using (Serilog.Context.LogContext.PushProperty(ClassNamePushProperty, typeof(T).Name))
        {
            _logger.LogCritical(exception, message);
        }
    }
}