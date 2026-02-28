using MassTransit;
using Microsoft.Extensions.Configuration;

namespace Nexticz.Lib.Shared.MassTransit.RabbitMq;

internal static class RabbitMqConfigurator
{
    public static void ConfigureRabbitMq(this IRabbitMqBusFactoryConfigurator configurator, IConfiguration configuration)
    {
        var rabbitMqSettings = configuration.GetSection("RabbitMq").Get<RabbitMqSettings>()
                               ?? throw new InvalidOperationException("RabbitMQ configuration section is missing.");
        
        configurator.Host(rabbitMqSettings.Host, "/", h =>
        {
            h.Username(rabbitMqSettings.Username);
            h.Password(rabbitMqSettings.Password);
        });
    }
}