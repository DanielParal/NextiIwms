using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Serilog;

namespace Nexticz.Lib.Shared.JsonConfigurations;

public static class ConfigurationBuilderExtensions
{
    public static IConfigurationBuilder AddJsonFileWithPolling(
        this IConfigurationBuilder builder,
        string path,
        bool optional = true,
        TimeSpan? reloadDelay = null)
    {
        reloadDelay ??= TimeSpan.FromSeconds(15);
        
        builder.AddJsonFile(path, optional, false);
        
        var jsonProvider = builder.Build()
            .Providers.OfType<JsonConfigurationProvider>()
            .FirstOrDefault(p => ((dynamic)p).Source?.Path == path);
          
        if (jsonProvider == null)
            return builder;
        
        _ = Task.Run(async () => await PollConfigFileAsync(path, jsonProvider, reloadDelay.Value));
        
        return builder;
    }
    
    private static async Task PollConfigFileAsync(string path, JsonConfigurationProvider jsonProvider, TimeSpan reloadDelay)
    {
        var lastWrite = File.GetLastWriteTimeUtc(path);

        while (true)
        {
            try
            {
                await Task.Delay(reloadDelay);
                var newWrite = File.GetLastWriteTimeUtc(path);
                if (newWrite == lastWrite) 
                    continue;
                
                lastWrite = newWrite;
                jsonProvider.Load();
                Log.Warning("Program.cs - {FilePath} reloaded at {ReloadedAt}", path, DateTime.UtcNow);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Program.cs - Error checking config file, FilePath: {FilePath}, ErrorMessage: {ErrorMessage}",
                    path, ex.Message);
            }
        }
    }
}