using Marten;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Nexticz.Nexty.Jipocar.Api;
using Nexticz.Module.Sign.Settings.Infrastructure;
using Nexticz.Module.Auth.Infrastructure.Common.Persistence;
using Testcontainers.MsSql;
using Testcontainers.PostgreSql;
using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;
using Nexticz.Lib.Shared.MassTransit;
using Nexticz.Lib.Shared.Testing;
using Testcontainers.RabbitMq;

namespace Nexticz.Module.Sign.SharedTesting;

public class SharedApiFactory(
    PostgreSqlContainer postgresContainer, 
    MsSqlContainer msSqlContainer,
    RabbitMqContainer rabbitMqContainer)
    : WebApplicationFactory<IAssemblyMarker>
{
    private readonly PostgreSqlContainer _dbPostgresSqlContainer = postgresContainer ?? throw new ArgumentNullException(nameof(postgresContainer));
    private readonly MsSqlContainer _dbMsSqlContainer = msSqlContainer ?? throw new ArgumentNullException(nameof(msSqlContainer));
    private readonly RabbitMqContainer _rabbitMqContainer = rabbitMqContainer ?? throw new ArgumentNullException(nameof(rabbitMqContainer));

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
            
            services.AddMarten<ISettingsDocumentStore>("Sign_Settings", _dbPostgresSqlContainer.GetConnectionString(), configuration);
            
            // MsSql setup
            services.RemoveAll(typeof(DbContextOptions<DataContext>));
            
            services.AddDbContext<DataContext>(options =>
                options.UseSqlServer(_dbMsSqlContainer.GetConnectionString()));

            services.AddMassTransitTestHarness(x =>
            {
                var registrars =
                    AppDomain.CurrentDomain
                        .GetAssemblies()
                        .SelectMany(a => a.GetTypes())
                        .Where(t => typeof(IConsumerRegistrar).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
                        .Select(Activator.CreateInstance)
                        .Cast<IConsumerRegistrar>();

                registrars.ToList().ForEach(r => r.Register(x));

                x.UsingRabbitMq((context, cfg) =>
                {
                    var uri = new Uri(_rabbitMqContainer.GetConnectionString());
                    cfg.Host(uri);
                    
                    cfg.ConfigureEndpoints(context);
                });
            });
            
            using var scope = services.BuildServiceProvider().CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
            dbContext.Database.Migrate();
        });

    }
}