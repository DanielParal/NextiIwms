using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Nexticz.Lib.Shared.UserProviders;
using Nexticz.Module.Auth.Application.Accounts.Orchestrators;
using Nexticz.Module.Auth.Application.Common.Behaviors;
using Nexticz.Module.Auth.Application.Common.Interfaces;
using Nexticz.Module.Auth.Application.Common.Services;
using Nexticz.Module.Auth.Application.FileHandling;
using Nexticz.Module.Auth.Application.MasstransitPublishers;
using Nexticz.Module.Auth.Application.MasstransitPublishers.EmailPublishers;

namespace Nexticz.Module.Auth.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddAuthApplication(this IServiceCollection services)
    {
        services
            .AddScoped<IAuthenticationService, AuthenticationService>()
            .AddScoped<ICurrentUserProvider, CurrentUserProvider>()
            .AddScoped<IAuthFileHandler, AuthFileHandler>()
            .AddScoped<IAuthEmailPublisher, AuthEmailPublisher>()
            .AddScoped<IAuthPublisher, AuthPublisher>()
            .AddScoped<ISyncAccountOrchestrator, SyncAccountOrchestrator>();

        services.AddMediatR(options =>
        {
            options.RegisterServicesFromAssemblyContaining(typeof(DependencyInjection));
            options.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });
        services.AddValidatorsFromAssemblyContaining(typeof(DependencyInjection));
        
        return services;
    }
}