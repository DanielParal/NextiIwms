using ErrorOr;
using Nexticz.OnPremise.Deployment.Cli.Configuration;
using Nexticz.OnPremise.Deployment.Cli.Logging;
using Nexticz.OnPremise.Deployment.Cli.Utilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Nexticz.OnPremise.Deployment.Cli.SourceControl;

internal class GitPuller : IGitPuller
{
    private readonly ExternalTool _externalTool;
    private readonly IEnhancedLogger<GitPuller> _logger;

    public GitPuller(
        IOptions<ExternalTool> externalTool,
        IEnhancedLogger<GitPuller> logger)
    {
        _externalTool = externalTool.Value;
        _logger = logger;
    }
    
    public async Task<ErrorOr<string>> DownloadLatestDockerComposeProject()
    {
        var result = await ProcessRunner.RunAsync(
            _externalTool.Git,
            "pull origin master",
            _logger);

        return result.IsError ? result : result.Value;
    }
}