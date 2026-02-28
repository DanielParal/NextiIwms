using Iwms_deployment_cli;
using Nexticz.OnPremise.Deployment.Cli.AzureLogin;
using Nexticz.OnPremise.Deployment.Cli.Configuration;
using Nexticz.OnPremise.Deployment.Cli.Coordinators;
using Nexticz.OnPremise.Deployment.Cli.Docker;
using Nexticz.OnPremise.Deployment.Cli.Logging;
using Nexticz.OnPremise.Deployment.Cli.SourceControl;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Context;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .CreateLogger();

var serviceCollection = new ServiceCollection()
    .AddSingleton<IEnvironmentExtractor, EnvironmentExtractor>()
    .AddSingleton<IAzureLoginRefresher, AzureLoginRefresher>()
    .AddSingleton<IDockerComposeRunner, DockerComposeRunner>()
    .AddSingleton(typeof(IEnhancedLogger<>), typeof(EnhancedLogger<>))
    .AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true))
    .AddCoordinates()
    .AddVersionUpdateStep()
    .AddFileBuildsStep();

serviceCollection.Configure<ExternalTool>(configuration.GetSection("ExternalTools"));
serviceCollection.Configure<AzureAcrConfig>(configuration.GetSection("AzureAcr"));
serviceCollection.Configure<FileConfig>(configuration.GetSection("FileConfiguration"));
serviceCollection.Configure<AzureKvConfig>(configuration.GetSection("AzureKeyVault"));
serviceCollection.Configure<EnvConfig>(configuration.GetSection("EnvConfiguration"));

var serviceProvider = serviceCollection.BuildServiceProvider();
var applicationCoordinator = serviceProvider.GetService<IApplicationCoordinator>();

// Set CorrelationId for logging in Serilog
using (LogContext.PushProperty("CorrelationId", Guid.NewGuid()))
{
    await applicationCoordinator!.Orchestrate();
    
    Console.WriteLine();
    Console.WriteLine("Press any key to exit...");
    Console.ReadKey();

    Log.CloseAndFlush();
}






