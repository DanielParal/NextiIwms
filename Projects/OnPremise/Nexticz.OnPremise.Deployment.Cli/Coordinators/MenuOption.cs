namespace Nexticz.OnPremise.Deployment.Cli.Coordinators;

public enum EMenuOption
{
    UpdateVersions = 1,
    Deployment = 2
}

public record MenuOption(int Id, EMenuOption EOption, string Description, string ConfirmMessage);
