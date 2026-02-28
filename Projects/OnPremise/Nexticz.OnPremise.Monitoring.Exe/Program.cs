
using Nexticz.OnPremise.Monitoring.Exe.DockerMonitoring;
using Nexticz.OnPremise.Monitoring.Exe.MonitoringRunner;
using Nexticz.OnPremise.Monitoring.Exe.ServerMonitoring;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(config =>
    {
        config.SetBasePath(Directory.GetCurrentDirectory());
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    })
    .UseSerilog((context, services, configuration) =>
    {
        configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services);
    })
    .ConfigureServices((context, services) =>
    {
        services.AddDockerMonitoring(context.Configuration);
        services.AddMonitoringRunner(context.Configuration);
        services.AddSingleton<IServerMonitoringHandler, ServerMonitoringHandler>();
    });

var host = builder.Build();
await host.RunAsync();