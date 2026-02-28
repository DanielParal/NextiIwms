using Microsoft.Extensions.DependencyInjection;
using Testcontainers.MsSql;
using Testcontainers.PostgreSql;

namespace Nexticz.Module.Portal.Tests.Functional;

public class PortalApiFactoryFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbPostgresSqlContainer = new PostgreSqlBuilder()
        .WithDatabase("testdb")
        .WithUsername("testuser")
        .WithPassword("testpass")
        .Build();

    private readonly MsSqlContainer _dbMsSqlContainer = new MsSqlBuilder()
        .WithPassword("yourStrong(!)Password")
        .Build();
    

    public PortalApiFactory Factory { get; protected set; }

    public PortalApiFactoryFixture()
    {
        Factory = new PortalApiFactory(
            _dbPostgresSqlContainer, _dbMsSqlContainer);
    }

    public virtual async Task InitializeAsync()
    {
        await Task.WhenAll(
            _dbPostgresSqlContainer.StartAsync(),
            _dbMsSqlContainer.StartAsync());
        
        using var scope = Factory.Services.CreateScope();
        await UsersDbSeeder.SeedAuthUsersAsync(scope);
    }

    public virtual async Task DisposeAsync()
    {
        await Factory.DisposeAsync();
        await _dbPostgresSqlContainer.StopAsync();
        await _dbMsSqlContainer.StopAsync();  
    }
}