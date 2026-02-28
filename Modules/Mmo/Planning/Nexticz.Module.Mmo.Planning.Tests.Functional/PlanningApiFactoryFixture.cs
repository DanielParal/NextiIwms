using Nexticz.Module.Mmo.SharedTesting;

namespace Nexticz.Module.Mmo.Planning.Tests.Functional;

public class PlanningApiFactoryFixture : SharedApiFactoryFixture
{
    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        
        using var client = Factory.CreateClient();
        await PlanningDbSeeder.SeedDataAsync(client);
    }
}