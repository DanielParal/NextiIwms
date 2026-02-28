using JasperFx;
using Microsoft.FeatureManagement;
using Nexticz.Module.Mmo.ModuleRegistrator;
using Nexticz.Module.Sign.ModuleRegistrator;
using Nexticz.Module.Vh.Presentation;
using Nexticz.Module.Auth.Application.Extensions;
using Nexticz.Module.Auth.Application.Middlewares;
using Nexticz.Module.Auth.Presentation;
using Nexticz.Module.Cuzk;
using Nexticz.Module.EmailSender;
using Nexticz.Module.Lang.Presentation;
using Nexticz.Module.Notifications;
using Nexticz.Module.Portal;
using Nexticz.Lib.Shared.Cors;
using Nexticz.Lib.Shared.DeploymentInfo;
using Nexticz.Lib.Shared.EndpointsConfiguration;
using Nexticz.Lib.Shared.FileHandling.Assets;
using Nexticz.Lib.Shared.HealthChecks;
using Nexticz.Lib.Shared.JsonConfigurations;
using Nexticz.Lib.Shared.Logging;
using Nexticz.Lib.Shared.MassTransit;
using Nexticz.Lib.Shared.Middlewares;
using Nexticz.Lib.Shared.Monitoring;
using Nexticz.Lib.Shared.Swagger;
using Nexticz.Lib.Shared.Testing;
using Nexticz.Lib.Shared.Time;
using Nexticz.Lib.Shared.Versioning;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Host.ApplyJasperFxExtensions();

builder.Configuration
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddJsonFile("iwms-be-api-secrets.json", optional: true, reloadOnChange: true)
    .AddJsonFile("iwms-be-api-deployment-info.json", optional: true, reloadOnChange: true)
    .AddJsonFileWithPolling("iwms-be-api-features.json");

builder.AddSerilog(builder.Configuration);

builder.Services.AddFeatureManagement();
builder.Services.AddMemoryCache();

builder.Services.AddEndpointsConfiguration()
    .AddSwaggerConfiguration()
    .AddApiVersioningConfiguration();

builder.Services.AddCors();
builder.Services.AddTimeInfo(builder.Configuration);
builder.Services.AddMassTransitConfiguration(builder.Configuration);
builder.Services.AddAssetsConfiguration(builder.Configuration);
builder.Services.AddDeploymentConfiguration(builder.Configuration);
builder.Services.AddMonitoringConfiguration(builder.Configuration);

builder.Services
    .AddVhModule(builder.Configuration)
    .AddAuthModule(builder.Configuration)
    .AddLangModule(builder.Configuration)
    .AddPortalModule(builder.Configuration)
    .AddNotificationsModule(builder.Configuration)
    .AddEmailSenderModule(builder.Configuration)
    .AddSignModule(builder.Configuration)
    .AddMmoModule(builder.Configuration)
    .AddCuzkModule(builder.Configuration);

builder.Services
    .AddAuthentication(builder.Configuration)
    .AddAuthorization(x => x.FallbackPolicy = null);

builder.Services.AddHealthChecks()
    .AddSignHealthChecks(builder.Configuration)
    .AddMmoHealthChecks(builder.Configuration);

builder.Services.AddHealthChecksUiConfiguration(builder.Configuration);

var app = builder.Build();

app.UseCorsConfiguration(builder.Configuration);
app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<AuthenticationAuthorizationMiddleware>();

app.UseHttpsRedirection();
await app.UseHealthChecksAsync(builder.Configuration);
app.UseCorrelationId();
app.UseTimeInfo();
await app.UseSwaggerConfigurationAsync();
app.UseDeploymentConfiguration();
app.UseMonitoringConfiguration();

app.UseVhModule();
app.UseLangModule();
app.UseAuthModule();
app.UsePortalModule();
app.UseMmoModule();
app.UseNotificationsModule();
app.UseSignModule();
app.UseEmailSenderModule();
await app.UseCuzkModuleAsync();

Log.Warning("Starting up IWMS API");

if (TestEnvironment.IsTesting(builder.Configuration))
{
    await app.RunAsync();
}
else
{
    await app.RunJasperFxCommands(args);
}

