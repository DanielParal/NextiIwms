using ErrorOr;

namespace Nexticz.OnPremise.Deployment.Cli.AzureLogin;

internal interface IAzureLoginRefresher
{
    Task<ErrorOr<string>> Login();
}