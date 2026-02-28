using JasperFx.Events.Projections;
using Marten;
using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;
using Nexticz.Module.Sign.DocumentManager.Domain.SigningDeviceAggregate;

namespace Nexticz.Module.Sign.DocumentManager.Infrastructure.SigningDevices;

internal class SigningDeviceConfigurator : IMartenConfigurator
{
    public void Configure(StoreOptions options)
    {
        options.Projections.Add<SigningDeviceProjection>(ProjectionLifecycle.Inline);
        options.Schema.For<SigningDevice>().DocumentAlias(MartenConfigurationOrchestrator.PluralizeTableName<SigningDevice>());
    }
}