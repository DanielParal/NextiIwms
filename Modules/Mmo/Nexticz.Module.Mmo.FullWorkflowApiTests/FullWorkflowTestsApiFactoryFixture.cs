using Nexticz.Module.Mmo.SharedTesting;

namespace Nexticz.Module.Mmo.FullWorkflowApiTests;

public class FullWorkflowTestsApiFactoryFixture : SharedApiFactoryFixture
{
    public FullWorkflowTestsDbSeeder.FullWorkflowApiSeedData FullWorkflowApiSeedData { get; private set; }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        
        using var client = Factory.CreateClient();
        FullWorkflowApiSeedData = await FullWorkflowTestsDbSeeder.SeedDataAsync(client);
    }
}