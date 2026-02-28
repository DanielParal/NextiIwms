using System.Diagnostics;
using ErrorOr;
using Nexticz.OnPremise.Deployment.Cli.Logging;
using Microsoft.Extensions.Logging;
using Exception = System.Exception;

namespace Nexticz.OnPremise.Deployment.Cli.Utilities;

internal static class ProcessRunner
{
    public static async Task<ErrorOr<string>> RunAsync<T>(
        string command, 
        string arguments,
        IEnhancedLogger<T> logger)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = command,
                Arguments = arguments,
                RedirectStandardOutput = true,  // Redirect output to capture it
                RedirectStandardError = true,   // Redirect errors to capture them
                UseShellExecute = false,     // Required for redirection
                CreateNoWindow = true        // Don't show a console window
            }
        };

        try
        {
            process.Start();
            
            var outputTask = process.StandardOutput.ReadToEndAsync();
            var errorTask = process.StandardError.ReadToEndAsync();
            
            await process.WaitForExitAsync();

            var output = await outputTask;
            var error = await errorTask;
            
            if (process.ExitCode != 0)
            {
                logger.LogError($"Error running command: {command}, with arguments: {arguments}. Error - {error}");
                return Error.Failure(
                    ErrorMessages.ProcessRunnerFailureCode, 
                    ErrorMessages.ProcessRunnerFailureDescription(command, arguments, error));
            }
            
            logger.LogInformation($"Processed command: {command}, with arguments: {arguments}.");
            return output.Trim();
        }
        catch (Exception ex)
        {
            logger.LogCritical($"Exception running command: {command}, with arguments: {arguments}. Exception message - {ex.Message}", ex);
            return Error.Failure(
                ErrorMessages.ProcessRunnerFailureUnexpectedCode, 
                ErrorMessages.ProcessRunnerFailureDescription(command, arguments, ex.Message));
        }
    }
}