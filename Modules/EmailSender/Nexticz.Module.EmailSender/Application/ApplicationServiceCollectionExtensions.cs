using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nexticz.Module.EmailSender.Application.EmailComposers;
using Nexticz.Module.EmailSender.Application.FileHandling;
using Nexticz.Module.EmailSender.Application.NotificationCollectors;
using Nexticz.Module.EmailSender.Application.Orchestrators;
using Nexticz.Module.EmailSender.Application.PipelineBehaviors;
using Nexticz.Module.EmailSender.Application.Publishers;

namespace Nexticz.Module.EmailSender.Application;

internal static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationLayer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(options =>
        {
            options.RegisterServicesFromAssemblyContaining(typeof(ApplicationServiceCollectionExtensions));
            options.AddOpenBehavior(typeof(EmailSenderValidationBehavior<,>));
            options.AddBehavior(typeof(IPipelineBehavior<,>), typeof(EmailSenderPostCommandBehavior<,>));
        });
        
        services.AddValidatorsFromAssemblyContaining(typeof(ApplicationServiceCollectionExtensions), includeInternalTypes: true);
        services.AddScoped<IEmailSenderNotificationCollector, EmailSenderNotificationCollector>();
        
        var emailSendersSettings
            = configuration.GetSection(nameof(EmailSendersSettings)).Get<EmailSendersSettings>()
              ?? new EmailSendersSettings();
        
        services.AddSingleton(emailSendersSettings);
        services.AddSingleton<IEmailComposer, EmailComposer>();
        
        services.AddScoped<ISendEmailOrchestrator, SendEmailOrchestrator>();
        services.AddScoped<IQueueEmailOrchestrator, QueueEmailOrchestrator>();
        
        services.AddScoped<IEmailConfirmationPublisher, EmailConfirmationPublisher>();
        services.AddScoped<IEmailSenderFileHandler, EmailSenderFileHandler>();
        
        return services;
    }
}