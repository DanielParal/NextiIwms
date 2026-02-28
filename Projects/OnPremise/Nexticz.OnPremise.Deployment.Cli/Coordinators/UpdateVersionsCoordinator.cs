using Nexticz.OnPremise.Deployment.Cli.AzureLogin;
using Nexticz.OnPremise.Deployment.Cli.Logging;
using Nexticz.OnPremise.Deployment.Cli.VersionUpdate;
using Microsoft.Extensions.Logging;

namespace Nexticz.OnPremise.Deployment.Cli.Coordinators;

internal class UpdateVersionsCoordinator : IUpdateVersionsCoordinator
{
    private readonly IAzureAcrVersionGetter _azAcrVersionGetter;
    private readonly IVersionBuilder _versionBuilder;
    private readonly IAzureLoginRefresher _azLoginRefresher;
    private readonly IEnhancedLogger<UpdateVersionsCoordinator> _logger;

    public UpdateVersionsCoordinator(
        IAzureAcrVersionGetter azAcrVersionGetter,
        IVersionBuilder versionBuilder,
        IAzureLoginRefresher azLoginRefresher,
        IEnhancedLogger<UpdateVersionsCoordinator> logger)
    {
        _azAcrVersionGetter = azAcrVersionGetter;
        _versionBuilder = versionBuilder;
        _azLoginRefresher = azLoginRefresher;
        _logger = logger;
    }

    public async Task<bool> UpdateVersionsAsync(string environment)
    {
        _logger.LogInformation($"{environment.ToUpper()}: updating token for azure.", true);
        var loginResult = await _azLoginRefresher.Login();

        if (loginResult.IsError)
        {
            _logger.LogError($"{environment.ToUpper()}: error refreshing token for Azure. Error: {loginResult.FirstError.Description}", true);
            return false;
        }
        
        _logger.LogInformation($"{environment.ToUpper()}: azure refreshed token successfully. Getting latest image versions", true);
        
        var versions = await _azAcrVersionGetter.GetLatestImageVersions();

        if (versions.IsError)
        {
            _logger.LogError($"{environment.ToUpper()}: error getting latest image versions. Error: {versions.FirstError.Description}", true);
            return false;
        }
        
        _logger.LogInformation($"{environment.ToUpper()}: image versions downloaded. Creating the files.", true);
        
        var numberOfFilesCreated = _versionBuilder.CreateVersionFiles(versions.Value);
        
        if (numberOfFilesCreated.IsError)
        {
            _logger.LogError($"{environment.ToUpper()}: error creating files. Error: {numberOfFilesCreated.FirstError.Description}", true);
            return false;
        }
        
        _logger.LogInformation($"{environment.ToUpper()}: created {numberOfFilesCreated.Value} files. Files created: {PrintCreatedFiles(versions.Value)}", true, true);
        return true;
    }

    private static string PrintCreatedFiles(Dictionary<string, string> createdFiles)
    {
        return string.Join(", ", createdFiles.Select(kvp => $"{kvp.Key}-{kvp.Value}"));
    }
}