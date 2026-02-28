namespace Nexticz.Module.Mmo.Settings.Application.Seeds.SeedHandlers;

internal interface ISeedRunner
{
    Task RunSeedAsync(CancellationToken cancellationToken);
}