namespace Nexticz.Module.Sign.Settings.Application.Seeds.Orchestrators;

internal interface ISeedOrchestrator
{
    Task SeedAsync(CancellationToken cancellationToken);
}