using JasperFx.Events.Projections;
using Marten;
using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;
using Nexticz.Module.Sign.Settings.Domain.SigningDeviceAggregate;

namespace Nexticz.Module.Sign.Settings.Infrastructure.SigningDevices;

internal class SigningDeviceConfigurator : IMartenConfigurator
{
    public void Configure(StoreOptions options)
    {
        options.Projections.Add<SigningDeviceProjection>(ProjectionLifecycle.Inline);
        
        options.Schema.For<SigningDevice>()
            .Index(x => x.Code, 
                idx =>
                {
                    idx.IsUnique = true;
                })
            .Index(x => x.LocationCode)
            .Index(x => x.PrinterCode);
        
        options.Schema.For<SigningDevice>().DocumentAlias(MartenConfigurationOrchestrator.PluralizeTableName<SigningDevice>());
    }
}