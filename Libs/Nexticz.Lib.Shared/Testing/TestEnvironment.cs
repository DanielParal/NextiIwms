using Microsoft.Extensions.Configuration;

namespace Nexticz.Lib.Shared.Testing;

public static class TestEnvironment
{
    private const string TestingFlagKey = "TestingFlag";

    public static bool IsTesting(IConfiguration configuration)
    {
        return configuration.GetValue<bool>(TestingFlagKey);
    }
    
    public static void SetTestingFlag(IConfigurationBuilder configBuilder)
    {
        configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
        {
            [TestingFlagKey] = "true"
        });
    }
}