using ErrorOr;
using Nexticz.OnPremise.Deployment.Cli.Configuration;
using Nexticz.OnPremise.Deployment.Cli.Logging;
using Nexticz.OnPremise.Deployment.Cli.Utilities;
using Microsoft.Extensions.Options;

namespace Nexticz.OnPremise.Deployment.Cli.Docker;

internal class DockerComposeRunner : IDockerComposeRunner
{
    private readonly ExternalTool _externalTool;
    private readonly FileConfig _fileConfig;
    private readonly IEnhancedLogger<DockerComposeRunner> _logger;

    public DockerComposeRunner(
        IOptions<FileConfig> fileConfig, 
        IOptions<ExternalTool> externalTool,
        IEnhancedLogger<DockerComposeRunner> logger)
    {
        _fileConfig = fileConfig.Value;
        _externalTool = externalTool.Value;
        _logger = logger;
    }
    
    public async Task<ErrorOr<string>> RunDockerComposeUp()
    {
        var arguments = $"-f \"{_fileConfig.RootDirectoryPath}\\docker-compose.yaml\" up -d";
        
        var result = await ProcessRunner.RunAsync(
            _externalTool.DockerCompose,
            arguments,
            _logger);

        return result.IsError ? result : result.Value;
    }
}