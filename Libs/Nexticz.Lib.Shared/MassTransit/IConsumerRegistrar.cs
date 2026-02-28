using MassTransit;

namespace Nexticz.Lib.Shared.MassTransit;

public interface IConsumerRegistrar
{
    void Register(IBusRegistrationConfigurator cfg);
}