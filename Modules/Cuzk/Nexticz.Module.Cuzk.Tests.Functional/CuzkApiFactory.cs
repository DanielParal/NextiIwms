using Marten;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Nexticz.Nexty.Jipocar.Api;
using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;
using Nexticz.Lib.Shared.Testing;
using Nexticz.Module.Auth.Infrastructure.Common.Persistence;
using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;
using Nexticz.Lib.Shared.Testing;
using Nexticz.Module.Cuzk.Infrastructure;
using Testcontainers.MsSql;
using Testcontainers.PostgreSql;

namespace Nexticz.Module.Cuzk.Tests.Functional;

public class CuzkApiFactory(
    PostgreSqlContainer postgresContainer, 
    MsSqlContainer msSqlContainer)
    : WebApplicationFactory<IAssemblyMarker>
{
    private readonly PostgreSqlContainer _dbPostgresSqlContainer = postgresContainer ?? throw new ArgumentNullException(nameof(postgresContainer));
    private readonly MsSqlContainer _dbMsSqlContainer = msSqlContainer ?? throw new ArgumentNullException(nameof(msSqlContainer));

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureLogging(logging =>
        {
            logging.ClearProviders();
        });
        
        builder.ConfigureAppConfiguration(config =>
        {
            TestEnvironment.SetTestingFlag(config);
        });
        
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IDocumentStore>();

            using var provider = services.BuildServiceProvider();
            var configuration = provider.GetRequiredService<IConfiguration>();
            
            services.AddMarten<ICuzkDocumentStore>("Cuzk", _dbPostgresSqlContainer.GetConnectionString(), configuration);
            
            // MsSql setup
            services.RemoveAll(typeof(DbContextOptions<DataContext>));
            
            services.AddDbContext<DataContext>(options =>
                options.UseSqlServer(_dbMsSqlContainer.GetConnectionString()));
            
            using var scope = services.BuildServiceProvider().CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
            dbContext.Database.Migrate();
        });

    }
}