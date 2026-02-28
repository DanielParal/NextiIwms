using JasperFx.Events.Projections;
using Marten;
using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;
using Nexticz.Module.Mmo.Washing.Domain.WashingMachineSpeedAggregate;

namespace Nexticz.Module.Mmo.Washing.Infrastructure.WashingMachineSpeeds;

internal class WashingMachineSpeedConfigurator : IMartenConfigurator
{
    public void Configure(StoreOptions options)
    {
        options.Projections.Add<WashingMachineSpeedProjection>(ProjectionLifecycle.Async);
        
        options.Schema.For<WashingMachineSpeed>().DocumentAlias(MartenConfigurationOrchestrator.PluralizeTableName<WashingMachineSpeed>());
    }
}