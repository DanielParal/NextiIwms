using Marten;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Nexticz.Nexty.Jipocar.Api;
using Nexticz.Module.Mmo.Drying.Infrastructure;
using Nexticz.Module.Mmo.Planning.Infrastructure;
using Nexticz.Module.Mmo.Reporting.Infrastructure;
using Nexticz.Module.Mmo.Settings.Infrastructure;
using Nexticz.Module.Mmo.Washing.Infrastructure;
using Nexticz.Module.Auth.Infrastructure.Common.Persistence;
using Testcontainers.MsSql;
using Testcontainers.PostgreSql;
using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;
using Nexticz.Lib.Shared.Testing;

namespace Nexticz.Module.Mmo.SharedTesting;

public class SharedApiFactory(PostgreSqlContainer postgresContainer, MsSqlContainer msSqlContainer)
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
            
            services.AddMarten<ISettingsDocumentStore>("Mmo_Settings", _dbPostgresSqlContainer.GetConnectionString(), configuration);
            services.AddMarten<IPlanningDocumentStore>("Mmo_Planning", _dbPostgresSqlContainer.GetConnectionString(), configuration);
            services.AddMarten<IWashingDocumentStore>("Mmo_Washing", _dbPostgresSqlContainer.GetConnectionString(), configuration);
            services.AddMarten<IDryingDocumentStore>("Mmo_Drying", _dbPostgresSqlContainer.GetConnectionString(), configuration);
            services.AddMarten<IReportingDocumentStore>("Mmo_Reporting", _dbPostgresSqlContainer.GetConnectionString(), configuration);
            
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