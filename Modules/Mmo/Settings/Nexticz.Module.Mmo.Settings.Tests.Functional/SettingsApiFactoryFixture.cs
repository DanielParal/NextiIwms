using Nexticz.Module.Mmo.SharedTesting;

namespace Nexticz.Module.Mmo.Settings.Tests.Functional;

public class SettingsApiFactoryFixture : SharedApiFactoryFixture
{
    public SettingsDbSeeder.ApiSeedData ApiSeedData { get; private set; }
    public SettingsApiFactoryFixture()
    {
        // Same factory initialization as base
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        using var client = Factory.CreateClient();
        ApiSeedData = await SettingsDbSeeder.SeedDataAsync(client);
    }
}