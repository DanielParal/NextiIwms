using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nexticz.Module.EmailSender.Application;
using Nexticz.Module.EmailSender.Infrastructure;

namespace Nexticz.Module.EmailSender;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEmailSenderModule(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddApplicationLayer(configuration);
        services.AddInfrastructureLayer(configuration);

        return services;
    }

    public static void UseEmailSenderModule(this WebApplication app)
    {
    }
}