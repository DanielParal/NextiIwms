namespace Nexticz.Module.Cuzk.Tests.Functional;

[CollectionDefinition(nameof(CuzkApiCollection))]
public class CuzkApiCollection : ICollectionFixture<CuzkApiFactoryFixture>
{
    // This class is a marker and requires no implementation.
    // It tells xUnit to share the same instance of SharedApiFactoryFixture across all tests in this collection.
}