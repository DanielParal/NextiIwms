using Microsoft.Extensions.DependencyInjection;
using Testcontainers.MsSql;
using Testcontainers.PostgreSql;
using Xunit;

namespace Nexticz.Module.Mmo.SharedTesting;

public abstract class SharedApiFactoryFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbPostgresSqlContainer = new PostgreSqlBuilder()
        .WithDatabase("testdb")
        .WithUsername("testuser")
        .WithPassword("testpass")
        .Build();

    private readonly MsSqlContainer _dbMsSqlContainer = new MsSqlBuilder()
        .WithPassword("yourStrong(!)Password")
        .Build();

    public SharedApiFactory Factory { get; protected set; }

    protected SharedApiFactoryFixture()
    {
        Factory = new SharedApiFactory(_dbPostgresSqlContainer, _dbMsSqlContainer);
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