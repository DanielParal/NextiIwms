using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;
using Nexticz.Module.EmailSender.Application.Interfaces;
using Nexticz.Module.EmailSender.Infrastructure.BaseRepositories;
using Nexticz.Module.EmailSender.Infrastructure.Smtp;

namespace Nexticz.Module.EmailSender.Infrastructure;

internal static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureLayer(
        this IServiceCollection services, IConfiguration configuration)
    {
        var emailSenderDbConnectionString = configuration.GetConnectionString("EmailSender") ??
                                            throw new InvalidOperationException("EmailSender PostgresDb connection string not found.");;
        services.AddMarten<IEmailSenderDocumentStore>("emailsender", emailSenderDbConnectionString, configuration);
        
        var smtpSettings
            = configuration.GetSection(nameof(SmtpSettings)).Get<SmtpSettings>()
              ?? new SmtpSettings();

        services.AddSingleton(smtpSettings);
        services.AddSingleton<IEmailSender, SmtpEmailSender>();
        
        services.AddScoped<IEmailSenderUnitOfWork, EmailSenderUnitOfWork>();
        services.AddScoped<IEmailSenderDocumentSessionProvider, EmailSenderDocumentSessionProvider>();
        services.AddScoped<IEmailSenderReadOnlyEventStoreRepository, EmailSenderReadOnlyEventStoreRepository>();
        
        services.AddHostedService<ScheduledEmailsSenderWorker>();
        
        return services;
    }
}