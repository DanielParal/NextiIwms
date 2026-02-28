using JasperFx.Events.Projections;
using Marten;
using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;
using Nexticz.Module.Mmo.Settings.Domain.WashingMachineEntity;


namespace Nexticz.Module.Mmo.Settings.Infrastructure.WashingMachines;

internal class WashingMachineConfigurator : IMartenConfigurator
{
    public void Configure(StoreOptions options)
    {
        options.Projections.Add<WashingMachineProjection>(ProjectionLifecycle.Inline);
        
        options.Schema.For<WashingMachine>().Index(x => x.Code, 
            idx =>
            {
                idx.IsUnique = true;
            });
        
        options.Schema.For<WashingMachine>().DocumentAlias(MartenConfigurationOrchestrator.PluralizeTableName<WashingMachine>());
    }
}