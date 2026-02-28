using JasperFx.Events.Projections;
using Marten;
using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;
using Nexticz.Module.Mmo.Reporting.Domain.WashingMachineSpeedAggregate;


namespace Nexticz.Module.Mmo.Reporting.Infrastructure.WashingMachineSpeeds;

internal class WashingMachineSpeedConfigurator : IMartenConfigurator
{
    public void Configure(StoreOptions options)
    {
        options.Projections.Add<WashingMachineSpeedProjection>(ProjectionLifecycle.Async);
        options.Schema.For<WashingMachineSpeed>().Index(x => new { x.Code, x.DateEnded });
        options.Schema.For<WashingMachineSpeed>().Index(x => x.DateStarted);
        options.Schema.For<WashingMachineSpeed>().Index(x => x.DateEnded);
        options.Schema.For<WashingMachineSpeed>().DocumentAlias(MartenConfigurationOrchestrator.PluralizeTableName<WashingMachineSpeed>());
    }
}