
namespace Nexticz.Module.Portal.Application.Seeds.Orchestrators;

internal interface ISeedOrchestrator
{
    Task SeedAsync(CancellationToken cancellationToken);
}