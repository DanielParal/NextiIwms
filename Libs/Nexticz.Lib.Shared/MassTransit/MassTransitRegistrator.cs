using MassTransit;
using MassTransit.MessageData;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nexticz.Lib.Shared.MassTransit.Filters;
using Nexticz.Lib.Shared.MassTransit.MessageData;
using Nexticz.Lib.Shared.MassTransit.RabbitMq;

namespace Nexticz.Lib.Shared.MassTransit;

public static class MassTransitRegistrator
{
    public static IServiceCollection AddMassTransitConfiguration(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        services.AddMassTransit(x =>
        {
            // register all consumers from all modules
            var registrars = 
                AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => typeof(IConsumerRegistrar).IsAssignableFrom(t) && t is { IsInterface: false, IsAbstract: false })
                .Select(t => (IConsumerRegistrar)Activator.CreateInstance(t)!);
            
            registrars.ToList().ForEach(r => r.Register(x));
            
            var messageDataConfiguration = configuration.GetSection(MessageDataSettings.SectionKey).Get<MessageDataSettings>();
            
            if (messageDataConfiguration is null)
                throw new InvalidOperationException("MessageDataSettings not found in configuration.");
            
            services.AddSingleton(messageDataConfiguration);
            services.AddHostedService<MessageDataCleanupWorker>();
            
            var messageDataRepository = new FileSystemMessageDataRepository(new DirectoryInfo(messageDataConfiguration.BaseFolder));
            services.AddSingleton<IMessageDataRepository>(messageDataRepository);
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.ConfigureRabbitMq(configuration);
                cfg.UseConsumeFilter(typeof(CorrelationIdLoggingFilter<>), context);
                
                cfg.UseMessageData(messageDataRepository);
                cfg.ConfigureEndpoints(context);
            });
        });
        
        return services;
    }
}