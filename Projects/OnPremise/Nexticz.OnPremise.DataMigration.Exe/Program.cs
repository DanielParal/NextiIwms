
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.FeatureManagement;
using Nexticz.OnPremise.DataMigration.Exe.AuthDataMigration;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(config =>
    {
        config.SetBasePath(Directory.GetCurrentDirectory());
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    })
    .ConfigureServices((context, services) =>
    {
        services.AddAuthDataMigration(context.Configuration);
        services.AddFeatureManagement();
    });

var host = builder.Build();
await host.RunAsync();