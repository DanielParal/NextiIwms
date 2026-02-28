namespace Nexticz.Module.Mmo.Settings.Tests.Functional;

[CollectionDefinition(nameof(SettingsApiCollection))]
public class SettingsApiCollection : ICollectionFixture<SettingsApiFactoryFixture>
{
    // This class is a marker and requires no implementation.
    // It tells xUnit to share the same instance of SharedApiFactoryFixture across all tests in this collection.
}