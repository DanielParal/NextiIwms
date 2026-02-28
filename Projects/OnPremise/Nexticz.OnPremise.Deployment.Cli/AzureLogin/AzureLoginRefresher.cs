
using Nexticz.OnPremise.Deployment.Cli.Configuration;
using Microsoft.Extensions.Options;
using ErrorOr;
using Nexticz.OnPremise.Deployment.Cli.Logging;
using Nexticz.OnPremise.Deployment.Cli.Utilities;
using Microsoft.Extensions.Logging;

namespace Nexticz.OnPremise.Deployment.Cli.AzureLogin;

internal class AzureLoginRefresher : IAzureLoginRefresher
{
    private readonly ExternalTool _externalTool;
    private readonly IEnhancedLogger<AzureLoginRefresher> _logger;

    public AzureLoginRefresher(
        IOptions<ExternalTool> externalTool,
        IEnhancedLogger<AzureLoginRefresher> logger)
    {
        _externalTool = externalTool.Value;
        _logger = logger;
    }
    
    public async Task<ErrorOr<string>> Login()
    {
        var command = _externalTool.AzCli;
        var arguments = "acr login --name jipocariwms";

        var result = await ProcessRunner.RunAsync(
                command,
                arguments,
                _logger);

        return result.IsError ? result : result.Value;
    }
}