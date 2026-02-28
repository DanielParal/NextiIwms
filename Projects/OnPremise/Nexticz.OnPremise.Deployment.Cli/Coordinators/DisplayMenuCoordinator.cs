using ErrorOr;
using Nexticz.OnPremise.Deployment.Cli.Logging;
using Nexticz.OnPremise.Deployment.Cli.Utilities;

namespace Nexticz.OnPremise.Deployment.Cli.Coordinators;

internal class DisplayMenuCoordinator : IDisplayMenuCoordinator
{
    private readonly IEnhancedLogger<DisplayMenuCoordinator> _logger;

    public DisplayMenuCoordinator(IEnhancedLogger<DisplayMenuCoordinator> logger)
    {
        _logger = logger;
    }
    
    public ErrorOr<MenuOption> GetOption(string environment)
    {
        DisplayInitialMessage(environment);
        var selectedOption = GetSelectedOption(environment);

        if (selectedOption.IsError)
        {
            return selectedOption;
        }
        
        return ConfirmMessage(selectedOption.Value) ? 
            selectedOption : 
            Error.Failure(ErrorMessages.DisplayMenuNotConfirmedCode, ErrorMessages.DisplayMenuNotConfirmedDescription);
    }

    private static void DisplayInitialMessage(string environment)
    {
        Console.WriteLine($"Welcome to {environment.ToUpper()} environment CLI tool.");
        Console.WriteLine("Select an option what do you want to do: ");
        Console.WriteLine("====================");
        Console.WriteLine();
    }

    private bool ConfirmMessage(MenuOption selectedOption)
    {
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(selectedOption.ConfirmMessage);
        Console.ResetColor();
        
        var userInput = Console.ReadLine();

        if (userInput?.ToLower() == "y")
        {
            return true;
        }
        
        _logger.LogWarning("You don't want to perform that action. We are exiting the CLI tool.");
        return false;
    }

    private ErrorOr<MenuOption> GetSelectedOption(string environment)
    {
        var options = new MenuOption[]
        {
            new (1, EMenuOption.UpdateVersions, "1. Update versions - this step will update image numbers from Azure container registries", $"{environment.ToUpper()} Your are about to update image numbers from Azure container registries. Are you sure you want to continue? (y/n)"),
            new (2, EMenuOption.Deployment, "2. Deployment - this step will execute all necessary steps to deploy new version of the application", $"{environment.ToUpper()} Your are about to deploy new version of application. Are you sure you want to continue? (y/n)")
            
        };
        
        DisplayMenu(options);

        while (true)
        {
            var userInput = Console.ReadLine();

            if (userInput?.ToLower().Trim() == "update")
            {
                return options[(int)EMenuOption.UpdateVersions - 1];
            }
            
            if (userInput?.ToLower().Trim() == "deploy")
            {
                return options[(int)EMenuOption.Deployment - 1];
            }
            
            _logger.LogWarning($"Invalid input - {userInput}. Please try again.", true);
        }

        return Error.Unexpected();
    }
    
    private static void DisplayMenu(MenuOption[] options)
    {
        foreach (var t in options)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(t.Description);
        }
        
        Console.ResetColor();

        Console.WriteLine();
        Console.WriteLine("Type 'update' for update versions or 'deploy' for deployment and press enter to continue. (update/deploy)");
    }
}