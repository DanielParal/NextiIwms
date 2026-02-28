using Nexticz.OnPremise.Deployment.Cli.Configuration;
using Nexticz.OnPremise.Deployment.Cli.Logging;

namespace Nexticz.OnPremise.Deployment.Cli.Coordinators;

internal class ApplicationCoordinator : IApplicationCoordinator
{
    private readonly IEnvironmentExtractor _environmentExtractor;
    private readonly IDisplayMenuCoordinator _displayMenuCoordinator;
    private readonly IUpdateVersionsCoordinator _updateVersionsCoordinator;
    private readonly IDeploymentCoordinator _deploymentCoordinator;
    private readonly IEnhancedLogger<ApplicationCoordinator> _logger;

    public ApplicationCoordinator(
        IEnvironmentExtractor environmentExtractor,
        IDisplayMenuCoordinator displayMenuCoordinator,
        IUpdateVersionsCoordinator updateVersionsCoordinator,
        IDeploymentCoordinator deploymentCoordinator,
        IEnhancedLogger<ApplicationCoordinator> logger)
    {
        _environmentExtractor = environmentExtractor;
        _displayMenuCoordinator = displayMenuCoordinator;
        _updateVersionsCoordinator = updateVersionsCoordinator;
        _deploymentCoordinator = deploymentCoordinator;
        _logger = logger;
    }
    
    public async Task Orchestrate()
    {
        _logger.LogInformation("Getting application environment");
        var environment = _environmentExtractor.GetEnvironment();

        if (environment.IsError)
        {
            _logger.LogError($"Error getting environment. Error description: {environment.FirstError.Description}", true);
            return;
        }

        _logger.LogInformation($"Selected environment: {environment.Value}. Selecting option");
        var selectedOption = _displayMenuCoordinator.GetOption(environment.Value);

        if (selectedOption.IsError)
        {
            _logger.LogError($"Error selecting option. Description: {selectedOption.FirstError.Description}");
            return;
        }

        _logger.LogInformation($"Selected option: {selectedOption.Value.EOption.ToString()}");
        switch (selectedOption.Value.EOption)
        {
            case EMenuOption.UpdateVersions:
                _logger.LogInformation("Starting updating versions.");
                await _updateVersionsCoordinator.UpdateVersionsAsync(environment.Value);
                break;
            case EMenuOption.Deployment:
                _logger.LogInformation("Starting deploying application.");
                await _deploymentCoordinator.DeployAsync(environment.Value);
                break;
            default:
                _logger.LogError("Error - selected unknown action.", true);
                break;
        }
    }
}