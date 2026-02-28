using ErrorOr;
using Nexticz.OnPremise.Deployment.Cli.Configuration;
using Nexticz.OnPremise.Deployment.Cli.Logging;
using Nexticz.OnPremise.Deployment.Cli.Utilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Nexticz.OnPremise.Deployment.Cli.VersionUpdate;

internal class AzureAcrVersionGetter : IAzureAcrVersionGetter
{
    private readonly ExternalTool _externalTool;
    private readonly AzureAcrConfig _azAcrConfig;
    private readonly IEnhancedLogger<AzureAcrVersionGetter> _logger;

    public AzureAcrVersionGetter(
        IOptions<ExternalTool> externalTool,
        IOptions<AzureAcrConfig> azAcrConfig,
        IEnhancedLogger<AzureAcrVersionGetter> logger)
    {
        _externalTool = externalTool.Value;
        _azAcrConfig = azAcrConfig.Value;
        _logger = logger;
    }
    
    public async Task<ErrorOr<Dictionary<string, string>>> GetLatestImageVersions()
    {
        var registryName = _azAcrConfig.RegistryName;
        var repositoryNames = _azAcrConfig.Repositories.Select(x => x.ImageName);
        var command = _externalTool.AzCli;

        var result = new Dictionary<string, string>();
        
        foreach (var repository in repositoryNames)
        {
            var arguments = $"acr repository show-tags --name {registryName} --repository {repository} --orderby time_desc --top 1 --output tsv";
            
            var processResult = await ProcessRunner.RunAsync(
                command,
                arguments,
                _logger);

            if (processResult.IsError)
            {
                return Error.Failure(processResult.FirstError.Code, processResult.FirstError.Description);
            }
            
            result.Add(repository, processResult.Value);
        }
        
        return result;
    }
}